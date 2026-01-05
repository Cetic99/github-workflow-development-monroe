using CashVault.Application.Interfaces;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.TicketAggregate;
using CashVault.Domain.Aggregates.TransactionAggregate;
using CashVault.Domain.TransactionAggregate;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class GetTransactionLogDetailsQueryValidator : AbstractValidator<GetTransactionLogDetailsQuery>
    {
        public GetTransactionLogDetailsQueryValidator()
        {
            RuleFor(p => p.Id).NotEmpty();
        }
    }

    public record GetTransactionLogDetailsQuery : IRequest<TransactionLogDetailsDto>
    {
        public int Id { get; set; }
    }

    internal sealed class GetTransactionLogDetailsQueryHandler : IRequestHandler<GetTransactionLogDetailsQuery, TransactionLogDetailsDto>
    {
        private readonly ITransactionRepository _db;
        private readonly IRegionalService _regionalService;
        private readonly ITerminal _terminal;

        public GetTransactionLogDetailsQueryHandler(ITransactionRepository db, IRegionalService regionalService, ITerminal terminal)
        {
            _db = db;
            _regionalService = regionalService;
            _terminal = terminal;
        }

        public async Task<TransactionLogDetailsDto> Handle(GetTransactionLogDetailsQuery request, CancellationToken cancellationToken)
        {
            var dto = new TransactionLogDetailsDto();
            var transaction = await _db.GetTransactionByIdAsync(request.Id);

            bool isCms = false;

            if (transaction is TicketTransaction ticketTransaction)
            {
                isCms = ticketTransaction?.TicketTypeDetail?.Code == TicketTypeDetail.Cms.Code;
            }

            var timeZone = _terminal.LocalTimeZone;

            if (transaction != null)
            {
                dto = new TransactionLogDetailsDto
                {
                    Id = transaction.Id,
                    Amount = transaction.Amount,
                    AmountRequested = transaction.AmountRequested,
                    PreviousCreditAmount = transaction.PreviousCreditAmount,
                    NewCreditAmount = transaction.NewCreditAmount,
                    Status = transaction.Status.Code,
                    Type = transaction.Type.Code,
                    ProcessingEnded = transaction.ProcessingEnded.HasValue ? _regionalService.ConvertToUserTimeZone(transaction.ProcessingEnded.Value, timeZone) : null,
                    ProcessingStarted = _regionalService.ConvertToUserTimeZone(transaction.ProcessingStarted, timeZone),
                    Description = transaction.Description,
                    ExternalReference = transaction.ExternalReference,
                    Currency = transaction.Currency,
                    Kind = transaction.GetType().Name,
                    IsCms = isCms
                };

                if (dto.Kind.Equals(nameof(DispenserBillTransaction)))
                {
                    var items = (transaction as DispenserBillTransaction).Items;

                    dto.DispenserBillItems = items.Select(x => new DispenserBillTransactionItemDto
                    {
                        Id = x.Id,
                        CassetteNumber = x.CassetteNumber,
                        BillDenomination = x.BillDenomination,
                        BillCountRequested = x.BillCountRequested,
                        BillCountRejected = x.BillCountRejected,
                        BillCountDispensed = x.BillCountDispensed
                    })
                    .ToList();
                }
            }

            return await Task.FromResult(dto);
        }
    }
}
