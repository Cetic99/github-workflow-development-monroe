using CashVault.Application.Features.DeviceFeatures.Commands;
using CashVault.Application.Features.DeviceFeatures.Queries;
using CashVault.Application.Features.DeviceFeatures.Queries.GetActiveDevicesQuery;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.OperatorAggregate;
using CashVault.WebAPI.Common.CustomAttributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashVault.WebAPI.Controllers;

[Route("api/[controller]")]
public class DeviceController : BaseController
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DeviceController(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    [Authorize]
    [HasPermission(PermissionEnum.Maintenance, PermissionEnum.MoneyService)]
    [HttpPut("{deviceType}/enable")]
    public async Task<IActionResult> EnableDevice(string deviceType)
    {
        DeviceType deviceTypeInstance = DeviceType.GetByCode<DeviceType>(deviceType.ToLowerInvariant());

        if (deviceTypeInstance == null)
        {
            return NotFound();
        }

        _ = Task.Run(async () =>
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(new EnableDeviceCommand(deviceTypeInstance));
        }, CancellationToken.None);

        return Accepted();
    }

    [Authorize]
    [HasPermission(PermissionEnum.Maintenance)]
    [HttpPut("{deviceType}/reset")]
    public async Task<IActionResult> ResetDevice(string deviceType)
    {
        DeviceType? deviceTypeInstance = DeviceType.GetByCode<DeviceType>(deviceType.ToLowerInvariant());

        if (deviceTypeInstance is null)
        {
            return NotFound();
        }

        _ = Task.Run(async () =>
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(new ResetDeviceCommand(deviceTypeInstance));
        });

        return Accepted();
    }

    [Authorize]
    [HasPermission(PermissionEnum.Maintenance, PermissionEnum.MoneyService)]
    [HttpPut("{deviceType}/disable")]
    public async Task<IActionResult> DisableDevice(string deviceType)
    {
        DeviceType deviceTypeInstance = DeviceType.GetByCode<DeviceType>(deviceType.ToLowerInvariant());

        if (deviceTypeInstance == null)
        {
            return NotFound();
        }

        _ = Task.Run(async () =>
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(new DisableDeviceCommand(deviceTypeInstance));
        });

        return Accepted();
    }

    [Authorize]
    [HasPermission(PermissionEnum.Maintenance)]
    [HttpGet("{deviceType}/diagnostic_commands")]
    public async Task<ActionResult<List<DeviceDiagnosticCommand>>> GetDeviceDiagnosticCommands(string deviceType)
    {
        DeviceType deviceTypeInstance = DeviceType.GetByCode<DeviceType>(deviceType.ToLowerInvariant());

        if (deviceTypeInstance == null)
        {
            return NotFound();
        }

        var result = await Mediator.Send(new GetDeviceDiagnosticCommandsQuery(deviceTypeInstance));

        return Ok(result);
    }

    [Authorize]
    [HasPermission(PermissionEnum.Maintenance)]
    [HttpPost("{deviceType}/diagnostic_command")]
    public async Task<ActionResult<List<DeviceDiagnosticCommand>>> RunDeviceDiagnosticCommand([FromRoute] string deviceType, [FromBody] RunDeviceDiagnosticCommand command)
    {
        DeviceType deviceTypeInstance = DeviceType.GetByCode<DeviceType>(deviceType.ToLowerInvariant());

        if (deviceTypeInstance == null)
        {
            return NotFound();
        }

        command.DeviceType = deviceTypeInstance;

        _ = Task.Run(async () =>
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(command);
        });

        return Accepted();
    }

    [HttpGet("/api/devices/active")]
    public async Task<ActionResult<List<ActiveDeviceDto>>> GetActiveDevices()
    {
        return await Mediator.Send(new GetActiveDevicesQuery());
    }

    [Authorize]
    [HasPermission(PermissionEnum.Maintenance)]
    [HttpGet("info/{deviceType}")]
    public async Task<ActionResult<DeviceInfoDto>> GetDeviceInfo([FromRoute] string deviceType)
    {
        return await Mediator.Send(new GetDeviceInfoQuery()
        {
            Type = deviceType
        });
    }

    [Authorize]
    [HasPermission(PermissionEnum.Maintenance)]
    [HttpPut("/api/devices/reset_all")]
    public async Task<ActionResult> ResetAllDevices()
    {
        _ = Task.Run(async () =>
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(new ResetAllDevicesCommand());
        });

        return Accepted();
    }
}
