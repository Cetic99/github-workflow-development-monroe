using System.Text.Json;
using CashVault.Application.Features.TicketFeatures.Commands;
using CashVault.Application.Features.TicketFeatures.Queries;
using CashVault.Domain.Aggregates.TicketAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashVault.WebAPI.Controllers;

[Route("api/betbox")]
public class BetboxController : BaseController
{
    [AllowAnonymous]
    [HttpGet("ticket/{barcode}/id/{id}/redeem")]
    public async Task<ActionResult<RedeemTicketDto>> RedeemTicket(string barcode, Guid id)
    {
        var result = await Mediator.Send(new RedeemTicketQuery() { Barcode = barcode, TicketType = TicketType.BetboxTicket, Id = id });

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("ticket/ack")]
    public async Task<ActionResult> HandleTicketAck(JsonDocument request)
    {
        var barcode = request.RootElement.GetProperty("barcode");

        if (string.IsNullOrEmpty(barcode.ToString()))
        {
            return BadRequest("Barcode is required");
        }

        var id = request.RootElement.GetProperty("id");

        if (string.IsNullOrEmpty(id.ToString()))
        {
            return BadRequest("Id is required");
        }

        return Ok(await Mediator.Send(new SendTicketRedemptionAckCommand(barcode.ToString(), TicketType.BetboxTicket, Guid.Parse(id.ToString()))));
    }

    [AllowAnonymous]
    [HttpPost("ticket/nack")]
    public async Task<ActionResult> HandleTicketNack(JsonDocument request)
    {
        var barcode = request.RootElement.GetProperty("barcode");

        if (string.IsNullOrEmpty(barcode.ToString()))
        {
            return BadRequest("Barcode is required");
        }

        var id = request.RootElement.GetProperty("id");

        if (string.IsNullOrEmpty(id.ToString()))
        {
            return BadRequest("Id is required");
        }

        return Ok(await Mediator.Send(new SendTicketRedemptionNackCommand(barcode.ToString(), TicketType.BetboxTicket, Guid.Parse(id.ToString()))));
    }
}
