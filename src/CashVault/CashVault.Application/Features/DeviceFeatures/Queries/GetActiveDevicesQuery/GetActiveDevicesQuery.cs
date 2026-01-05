using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries.GetActiveDevicesQuery
{
    public class GetActiveDevicesQuery : IRequest<List<ActiveDeviceDto>> { }

    internal sealed class GetActiveDevicesQueryHandler : IRequestHandler<GetActiveDevicesQuery, List<ActiveDeviceDto>>
    {
        private readonly ITerminal _terminal;

        public GetActiveDevicesQueryHandler(ITerminal terminal)
        {
            _terminal = terminal;
        }

        public async Task<List<ActiveDeviceDto>> Handle(GetActiveDevicesQuery request, CancellationToken cancellationToken)
        {
            var devices = _terminal.GetDevicesAsync();
            var result = new List<ActiveDeviceDto>();

            if (devices == null || devices.Count == 0)
            {
                return result;
            }

            foreach (var device in devices)
            {
                var deviceStatus = await device.GetCurrentStatus();
                result.Add(new ActiveDeviceDto
                {
                    Type = BaseHelper.GetDeviceTypeCode(device),
                    Name = device.Name,
                    Status = deviceStatus,
                    IsEnabled = device.IsEnabled,
                    IsConnected = device.IsConnected,
                    IsActive = device.IsActive
                });
            }

            return result;
        }
    }
}
