using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.TransactionAggregate;
using CashVault.Domain.TransactionAggregate;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class GetDailyMediaReportQueryValidator : AbstractValidator<GetDailyMediaReportQuery>
    {
        public GetDailyMediaReportQueryValidator()
        {
            RuleFor(x => x.Date).NotEmpty().WithMessage("Date is required");
        }
    }

    public record GetDailyMediaReportQuery : IRequest<DailyMediaReportDto>
    {
        public DateTime Date { get; set; }
    }

    internal sealed class GetDailyMediaReportQueryHandler : IRequestHandler<GetDailyMediaReportQuery, DailyMediaReportDto>
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly IEventLogRepository eventLogRepository;

        public GetDailyMediaReportQueryHandler(ITransactionRepository transactionRepository, IEventLogRepository eventLogRepository)
        {
            this.transactionRepository = transactionRepository;
            this.eventLogRepository = eventLogRepository;
        }

        public async Task<DailyMediaReportDto> Handle(GetDailyMediaReportQuery request, CancellationToken cancellationToken)
        {
            var dto = new DailyMediaReportDto();

            var (fromDate, toDate) = GetSelectedDateRange(request.Date);


            var billTransactions = await transactionRepository.GetBillTransactionsAync(toDate, fromDate);
            var ticketTransactions = await transactionRepository.GetTicketTransactionsAync(toDate, fromDate);

            var billsDispensed =
                billTransactions
                    .Where(x => x.GetType().Name == nameof(DispenserBillTransaction) &&
                               (x.Status.Code == TransactionStatus.Completed.Code ||
                                x.Status.Code == TransactionStatus.PartiallyCompleted.Code))
                    .Cast<DispenserBillTransaction>()
                    .ToList();

            var billsAccepted =
                billTransactions
                    .Where(x => x.GetType().Name == nameof(AcceptorBillTransaction) &&
                               (x.Status.Code == TransactionStatus.Completed.Code ||
                                x.Status.Code == TransactionStatus.PartiallyCompleted.Code))
                    .Cast<AcceptorBillTransaction>()
                    .ToList();

            var ticketTransactionsFiltered =
               ticketTransactions
                   .Where(x => x.Status.Code == TransactionStatus.Completed.Code ||
                               x.Status.Code == TransactionStatus.PartiallyCompleted.Code)
                   .ToList();

            // Bills

            dto.BillsDispensedValue = billsDispensed.Sum(x => x.Amount);
            foreach (var item in billsDispensed)
                dto.BillsDispensedCount += item.Items.Sum(x => x.BillCountDispensed);

            dto.BillsAcceptedCount = billsAccepted.Count();
            dto.BillsAcceptedValue = billsAccepted.Sum(x => x.Amount);

            foreach (var item in billsDispensed)
                dto.BillsRejectedByDispenserCount += item.Items.Sum(x => x.BillCountRejected);

            dto.BillsRejectedByAcceptorCount =
                await eventLogRepository.GetBillRejectedEventLogCountAsync(request.Date, request.Date);

            // Tickets

            var ticketsAccepted = ticketTransactionsFiltered.Where(x => x.Ticket != null && x.Ticket.IsStacked && TransactionType.Credit.Code.Equals(x.Type.Code, StringComparison.InvariantCultureIgnoreCase)).ToList();
            var ticketsPrinted = ticketTransactionsFiltered.Where(x => x.Ticket != null && x.Ticket.IsPrinted && TransactionType.Debit.Code.Equals(x.Type.Code, StringComparison.InvariantCultureIgnoreCase)).ToList();

            dto.TicketsAcceptedCount = ticketsAccepted.Count();
            dto.TicketsAcceptedValue = ticketsAccepted.Sum(x => x?.Ticket?.Amount ?? 0);

            dto.TicketsPrintedCount = ticketsPrinted.Count();
            dto.TicketsPrintedValue = ticketsPrinted.Sum(x => x?.Ticket?.Amount ?? 0);

            dto.TicketsRejectedByAcceptorCount =
                await eventLogRepository.GetTicketRejectedEventLogCountAsync(request.Date, request.Date);

            return await Task.FromResult(dto);
        }

        private static (DateTime fromDate, DateTime toDate) GetSelectedDateRange(DateTime dateTime)
        {
            DateTime fromDate = dateTime.Date;
            DateTime toDate = dateTime.Date.AddDays(1).AddTicks(-1);

            return (fromDate, toDate);
        }
    }
}
