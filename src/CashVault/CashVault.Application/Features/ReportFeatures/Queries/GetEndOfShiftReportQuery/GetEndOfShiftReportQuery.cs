using CashVault.Application.Interfaces;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.TransactionAggregate;
using CashVault.Domain.TransactionAggregate;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class GetEndOfShiftReportQueryValidator : AbstractValidator<GetEndOfShiftReportQuery>
    {
        public GetEndOfShiftReportQueryValidator()
        {
        }
    }

    public record GetEndOfShiftReportQuery : IRequest<EndOfShiftReportDto>
    {
    }

    internal sealed class GetEndOfShiftReportQueryHandler : IRequestHandler<GetEndOfShiftReportQuery, EndOfShiftReportDto>
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly IEventLogRepository eventLogRepository;
        private readonly IRegionalService regionalService;
        private readonly ITerminal terminal;

        public GetEndOfShiftReportQueryHandler(ITransactionRepository transactionRepository, IEventLogRepository eventLogRepository, ITerminal terminal, IRegionalService regionalService)
        {
            this.transactionRepository = transactionRepository;
            this.eventLogRepository = eventLogRepository;
            this.terminal = terminal;
            this.regionalService = regionalService;
        }

        public async Task<EndOfShiftReportDto> Handle(GetEndOfShiftReportQuery request, CancellationToken cancellationToken)
        {
            var dto = new EndOfShiftReportDto();

            var lastHarvestingEvent = await eventLogRepository.GetLastShiftMoneyHarvestedEventAsync();
            var fromDate = lastHarvestingEvent?.Created;
            var toDate = DateTime.UtcNow;

            dto.FromDate = fromDate.HasValue ? regionalService.ConvertToUserTimeZone(fromDate.Value, terminal.LocalTimeZone) : null;
            dto.ToDate = regionalService.ConvertToUserTimeZone(toDate, terminal.LocalTimeZone);

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
                await eventLogRepository.GetBillRejectedEventLogCountAsync(toDate, fromDate);

            // Tickets

            var ticketsAccepted = ticketTransactionsFiltered.Where(x => x.Ticket != null && x.Ticket.IsStacked && TransactionType.Credit.Code.Equals(x.Type.Code, StringComparison.InvariantCultureIgnoreCase)).ToList();
            var ticketsPrinted = ticketTransactionsFiltered.Where(x => x.Ticket != null && x.Ticket.IsPrinted && TransactionType.Debit.Code.Equals(x.Type.Code, StringComparison.InvariantCultureIgnoreCase)).ToList();

            dto.TicketsAcceptedCount = ticketsAccepted.Count();
            dto.TicketsAcceptedValue = ticketsAccepted.Sum(x => x?.Ticket?.Amount ?? 0);

            dto.TicketsPrintedCount = ticketsPrinted.Count();
            dto.TicketsPrintedValue = ticketsPrinted.Sum(x => x?.Ticket?.Amount ?? 0);

            dto.TicketsRejectedByAcceptorCount =
                await eventLogRepository.GetTicketRejectedEventLogCountAsync(toDate, fromDate);

            return await Task.FromResult(dto);
        }
    }
}
