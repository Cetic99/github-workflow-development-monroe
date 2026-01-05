using CashVault.Application.Features.DeviceFeatures.Queries;
using CashVault.Application.Features.OperatorFeatures.Commands;
using CashVault.Application.Features.OperatorFeatures.Queries.GetOperatorIdentificationCardsQuery;
using CashVault.Application.Features.OperatorFeatures.Queries.GetOperatorsQuery;
using CashVault.Application.Features.OperatorFeatures.Queries.GetPermissionsQuery;
using CashVault.Application.Features.ReportFeatures.Commands;
using CashVault.Domain.Aggregates.OperatorAggregate;
using CashVault.WebAPI.Common.CustomAttributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashVault.WebAPI.Controllers;

[Authorize]
[Route("api")]
public class OperatorController : BaseController
{
    [HasPermission(PermissionEnum.Administration)]
    [HttpGet("operators")]
    public async Task<OperatorsDto> GetOperators(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? firstName = null,
        [FromQuery] string? lastName = null)
    {
        return await Mediator.Send(new GetOperatorsQuery()
        {
            Page = page,
            PageSize = pageSize,
            FirstName = firstName,
            LastName = lastName
        });
    }

    [HasPermission(PermissionEnum.Administration)]
    [HttpGet("operator/{operatorId}/id-cards")]
    public async Task<OperatorIdCardsDto> GetOperatorIdentificationCards([FromRoute] int operatorId, [FromQuery] int page, [FromQuery] int pageSize)
    {
        return await Mediator.Send(new GetOperatorIdentificationCardsQuery()
        {
            OperatorId = operatorId,
            Page = page,
            PageSize = pageSize
        });
    }

    [HasPermission(PermissionEnum.Administration)]
    [HttpGet("permissions")]
    public async Task<PermissionsDto> GetPermissions()
    {
        return await Mediator.Send(new GetPermissionsQuery());
    }

    [HasPermission(PermissionEnum.Administration)]
    [HttpPut("operator")]
    public async Task<IActionResult> UpdateOperator([FromBody] UpdateOperatorCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HasPermission(PermissionEnum.Administration)]
    [HttpPost("operator")]
    public async Task<IActionResult> AddOperator([FromBody] AddOperatorCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HasPermission(PermissionEnum.Administration)]
    [HttpPut("operator/{operatorId}/id-card/{cardId}/activate")]
    public async Task<IActionResult> ActivateOperatorIdCard(
        [FromRoute] int operatorId,
        [FromRoute] int cardId,
        [FromBody] ActivateIdentificationCardCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HasPermission(PermissionEnum.Administration)]
    [HttpPut("operator/{operatorId}/id-card/{cardId}/deactivate")]
    public async Task<IActionResult> DeactivateOperatorIdCard(
        [FromRoute] int operatorId,
        [FromRoute] int cardId,
        [FromBody] DeactivateIdentificationCardCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HasPermission(PermissionEnum.Administration)]
    [HttpPut("operator/{operatorId}/id-card/{cardId}/block")]
    public async Task<IActionResult> ActivateOperatorIdCard(
        [FromRoute] int operatorId,
        [FromRoute] int cardId,
        [FromBody] BlockIdentificationCardCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HasPermission(PermissionEnum.Administration)]
    [HttpPut("operator/{operatorId}/password")]
    public async Task<IActionResult> SetOperatorPassword([FromBody] SetOperatorPasswordCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HasPermission(PermissionEnum.Reports)]
    [HttpPost("operator/harvest-shift-money")]
    public async Task<IActionResult> PrintHarvestShiftMoney()
    {
        try
        {
            var harvestShiftMoneyDto = await Mediator.Send(new GetEndOfShiftReportQuery());

            if (harvestShiftMoneyDto == null)
            {
                return NotFound();
            }

            await Mediator.Send(new PrintEndOfShiftReportCommand()
            {
                EndOfShiftReportDto = harvestShiftMoneyDto
            });

            return Ok(await Mediator.Send(new ClearEndOfShiftTotalsCommand()));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
