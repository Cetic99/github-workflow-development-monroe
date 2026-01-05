using System.Text.Json;
using CashVault.Application.Features.DeviceFeatures.Commands;
using CashVault.Application.Features.DeviceFeatures.Queries;
using CashVault.Domain.Aggregates.OperatorAggregate;
using CashVault.WebAPI.Common.CustomAttributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CashVault.WebAPI.Controllers
{
    [Authorize]
    [Route("api/configuration/terminal")]
    public class TerminalConfigurationController : BaseController
    {
        #region Ups

        [HasPermission(PermissionEnum.Configuration)]
        [HttpGet("ups")]
        public async Task<TerminalUPSConfigurationDto> GetUps()
        {
            return await Mediator.Send(new GetTerminalUPSConfigurationQuery());
        }

        [HasPermission(PermissionEnum.Configuration)]
        [HttpPost("ups")]
        public async Task<IActionResult> SaveUps(UpdateTerminalUPSConfiguration command)
        {
            return Ok(await Mediator.Send(command));
        }

        #endregion

        #region Network

        [HasPermission(PermissionEnum.Configuration)]
        [HttpGet("network")]
        public async Task<object> GetNetwork()
        {
            return await Mediator.Send(new GetTerminalNetworkConfigurationQuery());
        }

        [HasPermission(PermissionEnum.Configuration)]
        [HttpPost("network")]
        public async Task<IActionResult> SaveNetwork(UpdateTerminalNetworkConfiguration command)
        {
            return Ok(await Mediator.Send(command));
        }

        #endregion

        #region Online integrations

        [HasPermission(PermissionEnum.Configuration)]
        [HttpGet("online-integrations")]
        public async Task<object> GetOnlineIntegrations()
        {
            return await Mediator.Send(new GetTerminalOnlineIntegrationsConfigurationQuery());
        }

        [HasPermission(PermissionEnum.Configuration)]
        [HttpPost("online-integrations")]
        public async Task<IActionResult> SaveOnlneIntegrations(UpdateTerminalOnlineIntegrationsConfiguration command)
        {
            return Ok(await Mediator.Send(command));
        }

        #endregion

        #region Regional

        [AllowAnonymous]
        [HttpGet("regional")]
        public async Task<object> GetRegional()
        {
            return await Mediator.Send(new GetTerminalRegionalConfigurationQuery());
        }

        [HasPermission(PermissionEnum.Configuration)]
        [HttpPost("regional")]
        public async Task<IActionResult> SaveRegional(UpdateTerminalRegionalConfiguration command)
        {
            return Ok(await Mediator.Send(command));
        }

        #endregion

        #region Main

        [AllowAnonymous]
        [HttpGet("terminal-type-config")]
        public async Task<ActionResult<TerminalTypeConfigurationDto>> GetDevicesByTerminalTypes()
        {
            return await Mediator.Send(new GetTerminalTypeConfigurationQuery());
        }

        [HasPermission(PermissionEnum.Configuration)]
        [HttpGet("main")]
        public async Task<object> GetMain()
        {
            return await Mediator.Send(new GetTerminalMainConfigurationQuery());
        }

        [HasPermission(PermissionEnum.Configuration)]
        [HttpPost("main")]
        public async Task<IActionResult> SaveMain(UpdateTerminalMainConfiguration command)
        {
            return Ok(await Mediator.Send(command));
        }

        #endregion

        #region Server

        [HasPermission(PermissionEnum.Configuration)]
        [HttpGet("server")]
        public async Task<TerminalServerConfigurationDto> GetServer()
        {
            return await Mediator.Send(new GetTerminalServerConfigurationQuery());
        }

        [HasPermission(PermissionEnum.Configuration)]
        [HttpPost("server")]
        public async Task<IActionResult> SaveServer(UpdateTerminalServerConfiguration command)
        {
            return Ok(await Mediator.Send(command));
        }

        #endregion

        #region Bill acceptor

        [HasPermission(PermissionEnum.Configuration, PermissionEnum.BillAcceptor)]
        [HttpGet("bill-acceptor")]
        public async Task<JsonDocument> GetBillAcceptor([FromQuery] string? currency)
        {
            return await Mediator.Send(new GetBillAcceptorConfigurationQuery() { CurrencyIsoCode = currency });
        }

        [HasPermission(PermissionEnum.Configuration, PermissionEnum.BillAcceptor)]
        [HttpPost("bill-acceptor")]
        public async Task<IActionResult> SaveBillAcceptor([FromBody] JsonDocument payload)
        {
            return Ok(await Mediator.Send(new UpdateBillAcceptorConfigurationCommand(payload)));
        }

        [HasPermission(PermissionEnum.Configuration, PermissionEnum.BillAcceptor, PermissionEnum.MoneyService)]
        [HttpPut("bill-acceptor/empty")]
        public async Task<IActionResult> EmptyBillDispenserCassettes()
        {
            return Ok(await Mediator.Send(new EmptyBillTicketAcceptorCommand()));
        }

        [HasPermission(PermissionEnum.Configuration)]
        [HttpGet("coin-acceptor")]
        public async Task<JsonDocument?> GetCoinAcceptor()
        {
            return await Mediator.Send(new GetCoinAcceptorConfigurationQuery());
        }

        [HasPermission(PermissionEnum.Configuration)]
        [HttpPut("coin-acceptor")]
        public async Task<IActionResult> UpdateCoinAcceptorConfiguration([FromBody] JsonDocument payload)
        {
            return Ok(await Mediator.Send(new UpdateCoinAcceptorConfigurationCommand(payload)));
        }

        #endregion

        #region Bill dispenser

        [HasPermission(PermissionEnum.Configuration, PermissionEnum.BillDispenser)]
        [HttpGet("bill-dispenser")]
        public async Task<JsonDocument> GetBillDispenser()
        {
            return await Mediator.Send(new GetBillDispenserConfigurationQuery());
        }

        [HasPermission(PermissionEnum.Configuration, PermissionEnum.BillDispenser)]

        [HttpPost("bill-dispenser")]
        public async Task<IActionResult> SaveBillDispenser([FromBody] JsonDocument payload)
        {
            if (payload == null)
            {
                return BadRequest();
            }

            return Ok(await Mediator.Send(new UpdateBillDispenserConfigurationCommand(payload)));
        }

        [HasPermission(PermissionEnum.Configuration, PermissionEnum.BillDispenser, PermissionEnum.MoneyService)]
        [HttpPost("bill-dispenser-refill")]
        public async Task<IActionResult> RefillBillDispenser([FromBody] JsonDocument request)
        {
            var command = new RefillBillDispenserCommand
            {
                Cassettes = request.RootElement.GetProperty("cassettes").EnumerateArray().Select(
                    c => new RefillBillDispenserCommand.RefillBillCassette
                    {
                        CassetteNumber = c.GetProperty("cassetteNumber").GetInt32(),
                        BillCount = c.GetProperty("billCount").GetInt32()
                    }
                ).ToList()
            };
            return Ok(await Mediator.Send(command));
        }

        [HasPermission(PermissionEnum.Configuration, PermissionEnum.BillDispenser, PermissionEnum.MoneyService)]
        [HttpPut("bill-dispenser/empty-cassettes")]
        public async Task<IActionResult> EmptyBillDispenserCassettes(EmptyBillDispenserCassettesCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        #endregion

        #region Ticket printer

        [AllowAnonymous]
        [HttpGet("tito-printer")]
        public async Task<JsonDocument?> GetTiTOPrinter()
        {
            return await Mediator.Send(new GetTITOPrinterConfigurationQuery());
        }

        [HasPermission(PermissionEnum.Configuration)]
        [HttpPost("tito-printer")]
        public async Task<IActionResult> SaveTITOPrinterConfig(JsonDocument payload)
        {
            return Ok(await Mediator.Send(new UpdateTITOPrinterConfigurationCommand(payload)));
        }

        #endregion

        #region Sound

        [HasPermission(PermissionEnum.Configuration)]
        [HttpGet("customization/sound")]
        public async Task<object> GetSoundConfig()
        {
            return await Mediator.Send(new GetSoundConfigurationQuery());
        }

        [HasPermission(PermissionEnum.Configuration)]
        [HttpPost("customization/sound")]
        public async Task<IActionResult> SaveSoundConfig(UpdateSoundConfigurationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        #endregion

        #region Sound events

        [HasPermission(PermissionEnum.Configuration)]
        [HttpGet("customization/sound-events")]
        public async Task<SoundEventsConfigurationDto> GetSoundEventsConfig()
        {
            return await Mediator.Send(new GetSoundEventsConfigurationQuery());
        }

        [HasPermission(PermissionEnum.Configuration)]
        [HttpPost("customization/sound-events")]
        public async Task<IActionResult> SaveSoundEventsConfig(UpdateSoundEventsConfigurationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        #endregion

        #region Video

        [HasPermission(PermissionEnum.Configuration)]
        [HttpGet("customization/video")]
        public async Task<object> GetVideoConfig()
        {
            return await Mediator.Send(new GetDeviceInfoQuery());
        }

        [HasPermission(PermissionEnum.Configuration)]
        [HttpPost("customization/video")]
        public async Task<IActionResult> SaveVideoConfig(UpdateVideoConfigurationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        #endregion

        #region Flash

        [HasPermission(PermissionEnum.Configuration)]
        [HttpGet("customization/flash")]
        public async Task<object> GetFlashConfig()
        {
            return await Mediator.Send(new GetFlashConfigurationQuery());
        }

        [HasPermission(PermissionEnum.Configuration)]
        [HttpPost("customization/flash")]
        public async Task<IActionResult> SaveFlashConfig(UpdateFlashConfigurationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        #endregion

        #region User widgets

        [AllowAnonymous]
        [HttpGet("widgets")]
        public async Task<List<TerminalUserWidgetDto>> GetUserWidgets()
        {
            return await Mediator.Send(new GetTeminalUserWidgetsQuery());
        }

        [HasPermission(PermissionEnum.Configuration)]
        [HttpPut("widgets")]
        public async Task<IActionResult> UpdateUserWidgets(UpdateTerminalUserWidgetsConfigurationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HasPermission(PermissionEnum.Configuration)]
        [HttpGet("widgets/available")]
        public async Task<List<TerminalAvailableUserWidgetDto>> GetAvailableUserWidgets()
        {
            return await Mediator.Send(new GetTerminalAvailableUserWidgetsQuery());
        }

        #endregion
    }
}
