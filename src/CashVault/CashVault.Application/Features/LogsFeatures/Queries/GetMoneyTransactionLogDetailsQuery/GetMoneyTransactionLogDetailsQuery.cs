using CashVault.Application.Interfaces;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.LogsFeatures.Queries
{
    public record GetMoneyTransactionLogDetailsQuery : IRequest<MoneyTransactionLogDetailsDto>
    {
        public int Id { get; init; }
    }

    public class GetMoneyTransactionLogDetailsQueryValidator : AbstractValidator<GetMoneyTransactionLogDetailsQuery>
    {
        public GetMoneyTransactionLogDetailsQueryValidator()
        {
            RuleFor(x => x.Id).NotNull();
        }
    }

    internal sealed class GetMoneyTransactionLogDetailsQueryHandler : IRequestHandler<GetMoneyTransactionLogDetailsQuery, MoneyTransactionLogDetailsDto>
    {
        private readonly ITransactionRepository _db;
        private readonly IRegionalService _regionalService;
        private readonly ITerminal _terminal;

        public GetMoneyTransactionLogDetailsQueryHandler(ITransactionRepository db, IRegionalService regionalService, ITerminal terminal)
        {
            _db = db;
            _regionalService = regionalService;
            _terminal = terminal;
        }

        public async Task<MoneyTransactionLogDetailsDto> Handle(GetMoneyTransactionLogDetailsQuery request, CancellationToken cancellationToken)
        {
            var dto = new MoneyTransactionLogDetailsDto();
            var timeZone = _terminal.LocalTimeZone;

            MoneyStatusTransaction? transaction = await _db.GetMoneyStatusTransactionByIdAsync(request.Id);

            if (transaction != null)
            {
                dto = new MoneyTransactionLogDetailsDto
                {
                    Id = transaction.Id,
                    Amount = transaction.Amount,
                    Timestamp = _regionalService.ConvertToUserTimeZone(transaction.Timestamp, timeZone),
                    Status = transaction.Status.Code,
                    Type = transaction.Type.Code,
                    Currency = transaction.Currency,
                    Kind = transaction.GetType().Name,
                    DeviceType = transaction?.DeviceType?.Code,
                    OldDeviceBillAmount = transaction?.OldDeviceBillAmount ?? 0,
                    NewDeviceBillAmount = transaction?.NewDeviceBillAmount ?? 0
                };

                var oldDispenserMoneyStatus = transaction.OldDispenserBillCountStatus;
                var newDispenserMoneyStatus = transaction.NewDispenserBillCountStatus;

                if (oldDispenserMoneyStatus != null && newDispenserMoneyStatus != null)
                {
                    foreach (var cassette in oldDispenserMoneyStatus.Cassettes)
                    {
                        dto.DispenserMoneyStatus.Add(new BillDispenserCassetteItemDto
                        {
                            CassetteNumber = cassette.CassetteNumber,
                            BillDenomination = cassette.BillDenomination,
                            OldBillCount = cassette.CurrentBillCount,
                            NewBillCount = newDispenserMoneyStatus.Cassettes.Find(x => x.BillDenomination == cassette.BillDenomination)?.CurrentBillCount ?? 0
                        });
                    }
                }

                var oldAcceptorMoneyStatus = transaction.OldBillTicketAcceptorStackerStatus;
                var newAcceptorMoneyStatus = transaction.NewBillTicketAcceptorStackerStatus;

                dto.AcceptorMoneyStatus = new AcceptorItemDto
                {
                    OldBillCount = oldAcceptorMoneyStatus?.BillCount ?? 0,
                    NewBillCount = newAcceptorMoneyStatus?.BillCount ?? 0,
                    OldBillAmount = oldAcceptorMoneyStatus?.BillAmount ?? 0,
                    NewBillAmount = newAcceptorMoneyStatus?.BillAmount ?? 0,
                    OldTicketCount = oldAcceptorMoneyStatus?.TicketCount ?? 0,
                    NewTicketCount = newAcceptorMoneyStatus?.TicketCount ?? 0,
                    OldTicketAmount = oldAcceptorMoneyStatus?.TicketAmount ?? 0,
                    NewTicketAmount = newAcceptorMoneyStatus?.TicketAmount ?? 0
                };
            }

            return dto;
        }
    }
}
