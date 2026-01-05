using CashVault.Application.Interfaces;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using CashVault.Domain.ValueObjects;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public record GetMoneyServiceReportQuery : IRequest<MoneyServiceReportDto> { }

    internal sealed class GetMoneyServiceReportQueryHandler : IRequestHandler<GetMoneyServiceReportQuery, MoneyServiceReportDto>
    {
        private readonly IMoneyStatusRepository _moneyStatusRepository;
        private readonly IRegionalService _rs;

        public GetMoneyServiceReportQueryHandler(IMoneyStatusRepository moneyStatusRepository, IRegionalService rs)
        {
            _moneyStatusRepository = moneyStatusRepository;
            _rs = rs;
        }

        public async Task<MoneyServiceReportDto> Handle(GetMoneyServiceReportQuery request, CancellationToken cancellationToken)
        {
            var dto = new MoneyServiceReportDto();

            DispenserBillCountStatus dispenserBillStatus = await _moneyStatusRepository.GetDispenserBillCountStatusAsync();

            string casseteTranslation = _rs.Translate("Cassette");

            foreach (var cassetteBillStatus in dispenserBillStatus.Cassettes)
            {
                dto.DispenserCassettes.Add(new BillDispenserCassette
                {
                    Name = $"{casseteTranslation} #{cassetteBillStatus.CassetteNumber} - [{cassetteBillStatus.BillDenomination} {Currency.Default.Code}]",
                    BillDenomination = cassetteBillStatus.BillDenomination,
                    BillCount = cassetteBillStatus.CurrentBillCount,
                    CassetteNumber = cassetteBillStatus.CassetteNumber
                });
            }

            dispenserBillStatus.Cassettes.Sort((x, y) => x.CassetteNumber - y.CassetteNumber);

            var acceptorBillStatus = await _moneyStatusRepository.GetBillTicketAcceptorBillCountStatusAsync();
            var acceptorCoinStatus = await _moneyStatusRepository.GetCoinAcceptorCollectorStatusAsync();

            dto.AcceptorBillCount = acceptorBillStatus.BillCount;
            dto.AcceptorTicketCount = acceptorBillStatus.TicketCount;
            dto.AcceptorCoinCount = acceptorCoinStatus.CoinCount;

            dto.AcceptorBillAmount = acceptorBillStatus.BillAmount;
            dto.AcceptorTicketAmount = acceptorBillStatus.TicketAmount;
            dto.AcceptorCoinAmount = acceptorCoinStatus.CoinAmount;

            dto.DispenserRejectedBillsCount = dispenserBillStatus.RejectBin.BillCount;

            return dto;
        }
    }
}