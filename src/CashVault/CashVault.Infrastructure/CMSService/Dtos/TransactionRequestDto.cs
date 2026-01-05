using System.Text.Json.Serialization;
using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using CashVault.Domain.Common;
using CashVault.Domain.Common.Events;

namespace CashVault.Infrastructure.CMSService.Dtos;

internal class TransactionRequestDto : BaseDto
{
    public string TransactionCode { get; set; } = null!;
    public MoneyStatus MoneyStatus { get; set; } = null!;
    public UserInfoDto? User { get; set; }

    public TransactionRequestDto(string type, DateTime dateTime, string machineName, string secretKey, TransactionEvent transactionEvent, DispenserBillCountStatus dispenserBillCountStatus, BillTicketAcceptorStackerStatus acceptorMoneyStatus) : base(type, dateTime, machineName, secretKey)
    {
        ArgumentNullException.ThrowIfNull(transactionEvent, nameof(transactionEvent));
        ArgumentNullException.ThrowIfNull(dispenserBillCountStatus, nameof(dispenserBillCountStatus));

        Type = type;
        DateTime = dateTime;
        MachineName = machineName;
        TransactionCode = transactionEvent.GetType().Name;

        MoneyStatus = new MoneyStatus
        {
            Cassettes = dispenserBillCountStatus.Cassettes.Select(c => new DispenserCassetteStatus
            {
                CassetteNumber = c.CassetteNumber,
                CurrencyIsoCode = c.Currency.Code,
                BillDenomination = c.BillDenomination,
                CurrentBillCount = c.CurrentBillCount
            }).ToList(),
            AcceptorCassette = new AcceptorStackerStatus()
            {
                BillCount = acceptorMoneyStatus.BillCount,
                BillAmount = acceptorMoneyStatus.BillAmount,
                TicketCount = acceptorMoneyStatus.TicketCount,
                TicketAmount = acceptorMoneyStatus.TicketAmount
            }
        };

        User = new UserInfoDto()
        {
            Username = transactionEvent.CreatedByUser,
            FullName = transactionEvent.CreatedByUserFullName,
            Company = transactionEvent.CreatedByUserCompany
        };
    }
}

internal class MoneyStatus
{
    [JsonPropertyName("dispenserCassettes")]
    public List<DispenserCassetteStatus> Cassettes { get; set; } = [];
    [JsonPropertyName("acceptorCassette")]
    public AcceptorStackerStatus? AcceptorCassette { get; set; }
}

internal class DispenserCassetteStatus
{
    public int CassetteNumber { get; set; }
    public string? CurrencyIsoCode { get; set; }
    public int BillDenomination { get; set; }
    public int CurrentBillCount { get; set; }
}

internal class AcceptorStackerStatus
{
    public int BillCount { get; set; }
    public decimal BillAmount { get; set; }
    public int TicketCount { get; set; }
    public decimal TicketAmount { get; set; }
}


