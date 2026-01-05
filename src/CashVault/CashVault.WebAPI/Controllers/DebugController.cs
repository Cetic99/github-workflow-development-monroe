using CashVault.Application.Features.CreditFeatures.Commands;
using CashVault.Application.Features.OperatorFeatures.Commands;
using CashVault.Application.Features.TicketFeatures.Commands;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Events;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.TicketAggregate;
using CashVault.Domain.Aggregates.TransactionAggregate.Events;
using CashVault.Infrastructure.PersistentStorage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CashVault.WebAPI.Controllers
{
    [ApiController]
    [Route("debug")]
    public class DebugController : BaseController
    {
        private readonly IMediator mmediator;
        private readonly ICMSService _cmsProtocol;
        private readonly ITerminal terminal;

        public DebugController(CashVaultContext db, IMediator mediator, ICMSService cmsProtocol, ITerminal terminal)
        {
            mmediator = mediator;
            _cmsProtocol = cmsProtocol;
            this.terminal = terminal;
        }

        [HttpGet("test-bill-dispensing")]
        public async Task<IActionResult> TestBillDispensing()
        {
            var result = await mmediator.Send(new DispenseBillsCommand(Guid.NewGuid(), 30, null));
            return Ok($"Requested: {30} Dispensed: {result}");
        }

        [HttpGet("test-redeem-ticket")]
        public async Task<IActionResult> TestRedeemTicket(string barcode)
        {
            var result = await mmediator.Send(new RedeemTicketCommand { Barcode = barcode, TicketType = TicketType.TITO });
            return result ? Ok("Success") : BadRequest("Failed");
        }

        [HttpGet("test-accepted-ticket")]
        public async Task<IActionResult> TestAcceptedTicket(string barcode)
        {
            await mmediator.Publish(new TicketAcceptedEvent(barcode));
            return Ok("Event published");
        }

        [HttpGet("test-process-ticket")]
        public async Task<IActionResult> TestProcessTicket(string barcode)
        {
            var result = await mmediator.Send(new RedeemTicketCommand { Barcode = barcode, TicketType = TicketType.TITO });
            await mmediator.Publish(new TicketAcceptedEvent(barcode));
            return result ? Ok("Success") : BadRequest("Failed");
        }

        [HttpPost("test-create-operator")]
        public async Task<IActionResult> TestCreateOperator(AddOperatorCommand command)
        {
            return Ok(await mmediator.Send(command));
        }

        [HttpGet("test-device-error")]
        public async Task<IActionResult> TestDeviceError(string message)
        {
            await mmediator.Publish(new DeviceErrorOccuredEvent(Domain.Aggregates.DeviceAggregate.DeviceType.BillDispenser, "error occured on bill dispenser device"));
            return Ok("Event published");
        }

        #region CMS Integration
        [HttpGet("test-redeem-ticket-cms")]
        public async Task<IActionResult> TestRedeemTicketCMS(string barcode)
        {
            var result = await _cmsProtocol.RedeemTicket(barcode);
            return result != null ? Ok(result) : BadRequest("Failed");
        }

        [HttpGet("test-complete-redeem-ticket-cms")]
        public async Task<IActionResult> TestCompleteRedeemTicketCMS(string barcode)
        {
            var result = await _cmsProtocol.CompleteRedeemTicket(barcode);
            return result ? Ok("Success") : BadRequest("Failed");
        }

        [HttpGet("test-fail-redeem-ticket-cms")]
        public async Task<IActionResult> TestFailRedeemTicketCMS(string barcode)
        {
            var result = await _cmsProtocol.FailTicketRedemption(barcode);
            return result ? Ok("Success") : BadRequest("Failed");
        }

        [HttpGet("test-print-ticket-cms")]
        public async Task<IActionResult> TestPrintTicketCMS(decimal amount)
        {
            var result = await _cmsProtocol.RequestTicketPrinting(amount);
            return result != null ? Ok(result) : BadRequest("Failed");
        }

        [HttpGet("test-complete-print-ticket-cms")]
        public async Task<IActionResult> TestCompletePrintTicketCMS(string barcode)
        {
            var result = await _cmsProtocol.CompleteTicketPrinting(barcode);
            return result ? Ok("Success") : BadRequest("Failed");
        }

        [HttpGet("test-fail-print-ticket-cms")]
        public async Task<IActionResult> TestFailPrintTicketCMS(string barcode)
        {
            var result = await _cmsProtocol.FailTicketPrinting(barcode);
            return result ? Ok("Success") : BadRequest("Failed");
        }

        //[HttpGet("test-send-event-cms")]
        //public async Task<IActionResult> TestSendEventCMS()
        //{
        //    var @event = new DeviceActivatedEvent("1234567890", 100);
        //    var result = await _cmsProtocol.SendEvent(@event);
        //    return result ? Ok("Success") : BadRequest("Failed");

        //}
        #endregion


        [HttpGet("test-terminal")]
        public async Task<IActionResult> TestNewTerminalClass()
        {
            var tterminal = terminal;

            return Ok("Event published");
        }

        [HttpGet("test-nfc-login")]
        public async Task<IActionResult> TestNfcLogin(Guid cardUuid, string cardUID)
        {
            var command = new CardAuthenticatedEvent(cardUuid, cardUID);
            await mmediator.Publish(command);
            return Accepted();
        }
    }
}
