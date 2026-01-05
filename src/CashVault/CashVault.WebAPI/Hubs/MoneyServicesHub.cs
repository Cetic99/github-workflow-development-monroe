using System.Text.Json;
using CashVault.Application.Features.CreditFeatures.Commands;
using CashVault.Application.Features.CreditFeatures.Queries;
using CashVault.Application.Features.TicketFeatures.Commands;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using CashVault.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CashVault.WebAPI.Hubs;

public class MoneyServicesHub : Hub<IMoneyServicesHub>
{
    private class ReceiveMessagePayload
    {
        public string messageType { get; set; }
    }

    private class PayoutRequestedPayload
    {
        public Guid requestId { get; set; }
        public decimal ticketAmount { get; set; }
        public List<BillDenomSpecification>? billSpecification { get; set; }
    }

    private class BillDenomSpecification
    {
        public int count { get; set; }
        public int denomination { get; set; }
    }


    private readonly IMediator mediator;
    private readonly ILogger<MoneyServicesHub> logger;
    private readonly ITerminal terminal;
    public MoneyServicesHub(IMediator mediator, ILogger<MoneyServicesHub> logger, ITerminal terminal)
    {
        this.mediator = mediator;
        this.logger = logger;
        this.terminal = terminal;
    }

    public async Task ReceiveMessage(string area, string jsonMessage)
    {
        try
        {
            var payloadObject = JsonSerializer.Deserialize<ReceiveMessagePayload>(jsonMessage);
            string? messageType = payloadObject?.messageType;

            if (messageType == "GetCreditsAmount")
            {
                await GetCreditsAmount();
            }
            else if (messageType == "PayoutRequested")
            {
                await PayoutRequested(jsonMessage);
            }
            else
            {
                await Clients.All.SendMessage("ErrorMessage", "Unknown message type received");
            }
        }
        catch (Exception)
        {
            await Clients.All.SendMessage("ErrorMessage", "Global error");
        }
    }

    public async Task MoneyStatusError(string errorMessage)
    {
        await Clients.All.SendMessage("ErrorMessage", errorMessage);
    }

    private async Task GetCreditsAmount()
    {
        var result = await mediator.Send(new GetCurrentCreditQuery());
        var amount = BaseHelper.RoundNumber(result.CreditAmount, terminal.AmountPrecision);

        string payload = JsonSerializer.Serialize(new
        {
            amount,
            currency = result.Currency.Code,
            currencySymbol = result.Currency.Symbol,
            amountPrecision = terminal.AmountPrecision
        });

        await Clients.All.SendMessage("SetCreditsAmount", payload);
    }

    private async Task PayoutRequested(string jsonPayload)
    {
        decimal ticketAmountPayedOut = 0;
        decimal cashAmountPayedOut = 0;

        try
        {
            PayoutRequestedPayload? payloadObject = JsonSerializer.Deserialize<PayoutRequestedPayload>(jsonPayload);

            if (payloadObject == null)
            {
                await Clients.All.SendMessage("PayoutFailed", "Invalid payload received");
                return;
            }

            Guid requestId = payloadObject.requestId;
            decimal ticketAmount = payloadObject.ticketAmount;
            decimal cashAmount = (int)(payloadObject.billSpecification?.Sum(x => x.denomination * x.count) ?? 0);

            bool ticketPrintingResult = false;
            decimal billDispensingResult = 0;


            if (cashAmount > 0)
            {
                billDispensingResult = await BillDispensingRequested(requestId, cashAmount, payloadObject.billSpecification);
            }

            if (ticketAmount > 0)
            {
                ticketPrintingResult = await TicketPrintingRequested(requestId, ticketAmount);
            }

            ticketAmountPayedOut = ticketAmount > 0 && ticketPrintingResult ? ticketAmount : 0;
            cashAmountPayedOut = cashAmount > 0 && billDispensingResult > 0 ? billDispensingResult : 0;

            string payload = JsonSerializer.Serialize(new
            {
                cashAmountRequested = cashAmount,
                ticketAmountRequested = ticketAmount,
                ticketAmountPayedOut,
                cashAmountPayedOut,
                requestId
            });

            if (ticketAmount > 0 && ticketPrintingResult && cashAmount > 0 && billDispensingResult > 0)
            {
                await Clients.All.SendMessage("PayoutCompleted", payload);
            }
            else if (ticketAmount > 0 && ticketPrintingResult && cashAmount == 0)
            {
                await Clients.All.SendMessage("PayoutCompleted", payload);
            }
            else if (cashAmount > 0 && billDispensingResult > 0 && ticketAmount == 0)
            {
                await Clients.All.SendMessage("PayoutCompleted", payload);
            }
            else
            {
                await Clients.All.SendMessage("PayoutFailed", payload);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "PayoutRequested failed, reason: " + ex.Message);

            await Clients.All.SendMessage("PayoutFailed", JsonSerializer.Serialize(new
            {
                ticketAmountPayedOut,
                cashAmountPayedOut,
            }));
        }
    }

    private async Task<bool> TicketPrintingRequested(Guid requestId, decimal amount)
    {
        try
        {
            var result = await mediator.Send(new PrintTicketCommand()
            {
                AmountRequested = amount,
                requestId = requestId
            });

            if (result)
            {
                await Clients.All.SendMessage("TicketPrintingCompleted", JsonSerializer.Serialize(new
                {
                    amountRequested = amount,
                    amountPayedOut = amount,
                    requestId
                }));
                return true;
            }
            else
            {
                await Clients.All.SendMessage("TicketPrintingFailed", JsonSerializer.Serialize(new
                {
                    reason = "Failed",
                    amountRequested = amount,
                    amountPayedOut = 0.0,
                    requestId
                }));

                return false;
            }
        }
        catch (Exception ex)
        {
            await Clients.All.SendMessage("TicketPrintingFailed", JsonSerializer.Serialize(new
            {
                reason = ex.Message,
                amountRequested = amount,
                amountPayedOut = 0.0,
                requestId
            }));

            return false;
        }
    }

    private async Task<decimal> BillDispensingRequested(Guid requestId, decimal amount, List<BillDenomSpecification>? billSpecification)
    {
        try
        {
            var result = await
                mediator.Send(new DispenseBillsCommand
                    (requestId, amount, billSpecification?.Select(x => new DenominationCount(x.denomination, x.count)).ToList()));

            if (result > 0)
            {
                await Clients.All.SendMessage("BillDispensingCompleted", JsonSerializer.Serialize(new
                {
                    amountRequested = amount,
                    amountPayedOut = result,
                    requestId
                }));

                return result;
            }
            else
            {
                await Clients.All.SendMessage("BillDispensingFailed", JsonSerializer.Serialize(new
                {
                    reason = "Failed",
                    amountRequested = amount,
                    amountPayedOut = 0.0,
                    requestId
                }));
            }
        }
        catch (Exception ex)
        {
            await Clients.All.SendMessage("BillDispensingFailed", JsonSerializer.Serialize(new
            {
                reason = ex.Message,
                amountRequested = amount,
                amountPayedOut = 0.0,
                requestId
            }));
        }

        return 0;
    }
}

public interface IMoneyServicesHub
{
    Task ReceiveMessage(string area, string jsonMessage);
    Task SendMessage(string messageType, string payload);
}
