using CashVault.Application.Features.CreditFeatures.Queries;
using Microsoft.AspNetCore.Mvc;

namespace CashVault.WebAPI.Controllers;

[Route("api/[controller]")]
public class MoneyController : BaseController
{
    [HttpGet("dispense_options")]
    public async Task<ActionResult<PosibleBillsToDispenseDto>> GetPosibleBillsToDispenseQuery()
    {
        return Ok(await Mediator.Send(new GetPosibleBillsToDispenseQuery()));
    }
}
