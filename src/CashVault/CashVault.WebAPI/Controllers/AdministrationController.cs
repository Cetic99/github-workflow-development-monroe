using CashVault.Application.Features.AdministrationFeatures.Queries;
using CashVault.Application.Features.DeviceFeatures.Commands;
using CashVault.Application.Features.DeviceFeatures.Queries;
using CashVault.Domain.Aggregates.OperatorAggregate;
using CashVault.WebAPI.Common.CustomAttributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashVault.WebAPI.Controllers;

[Route("api/[controller]")]
public class AdministrationController : BaseController
{
    #region Messages
    [Authorize]
    [HasPermission(PermissionEnum.Administration)]
    [HttpGet("messages")]
    public async Task<MessagesDto> GetMessages(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] string? key,
            [FromQuery] string? value,
            [FromQuery] string? languageCode)
    {
        return await Mediator.Send(new GetMessagesQuery()
        {
            Page = page,
            PageSize = pageSize,
            Key = key,
            Value = value,
            LanguageCode = languageCode
        });
    }

    [HttpGet("all-messages")]
    public async Task<AllMessagesDto> GetAllMessages()
    {
        return await Mediator.Send(new GetAlllMessagesQuery());
    }

    [Authorize]
    [HasPermission(PermissionEnum.Administration)]
    [HttpPut("message")]
    public async Task<IActionResult> SaveMessage(UpdateMessageCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
    #endregion

    #region Payout rules

    [Authorize]
    [HasPermission(PermissionEnum.Administration)]
    [HttpGet("payout-rules")]
    public async Task<PayoutRulesDto> GetPayoutRules()
    {
        return await Mediator.Send(new GetPayoutRulesQuery());
    }

    [Authorize]
    [HasPermission(PermissionEnum.Administration)]
    [HttpPut("payout-rules")]
    public async Task<IActionResult> SavePayoutRules(UpdatePayoutRulesCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    #endregion
}