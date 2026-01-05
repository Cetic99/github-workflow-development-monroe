using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common;

namespace CashVault.WebAPI.Services
{
    public class HeartbeatService : IHostedService
    {
        private readonly INotificationService _notificationService;
        private readonly ITerminal _terminal;
        private readonly IAppInfoService _appInfoService;
        private Timer? _timer;

        public HeartbeatService(ITerminal terminal, INotificationService notificationService, IAppInfoService appInfoService)
        {
            _notificationService = notificationService;
            _terminal = terminal;
            _appInfoService = appInfoService;
        }

        private async Task SendDataAsync(object? state)
        {
            var model = new HeartbeatModel();

            model.AppVersion = _appInfoService.Version;
            model.Mode = _terminal?.OperatingMode?.Id ?? TerminalOperatingMode.UnknownUser.Id;
            var devices = _terminal?.GetDevicesAsync();


            if (devices?.Any() ?? false)
                foreach (var device in devices)
                {
                    var status = await device.GetCurrentStatus();
                    model.Devices.Add(new DeviceStateModel()
                    {
                        Type = BaseHelper.GetDeviceTypeCode(device),
                        Name = device.Name,
                        Status = status,
                        IsEnabled = device.IsEnabled,
                        IsConnected = device.IsConnected,
                        IsActive = device.IsActive,
                        Warning = device.GetWarning(),
                        Error = device.GetError(),
                        CommandInProgress = device.CommandInProgress
                    });
                }

            if (_notificationService != null)
                _notificationService.SendHeartbeat(model);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(500));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void TimerCallback(object? state)
        {
            _ = SendDataAsync(state); // Fire and forget the async task
        }
    }
}