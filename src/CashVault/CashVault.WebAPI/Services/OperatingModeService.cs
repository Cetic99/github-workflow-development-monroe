using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common;

namespace CashVault.WebAPI.Services
{
    public class OperatingModeService : IHostedService
    {
        private readonly INotificationService _notificationService;
        private readonly ITerminal _terminal;
        private Timer? _timer;

        public OperatingModeService(ITerminal terminal, INotificationService notificationService)
        {
            _notificationService = notificationService;
            _terminal = terminal;
        }

        private void SendData(object? state)
        {
            var model = new HeartbeatModel();
            model.Mode = _terminal?.OperatingMode?.Id ?? TerminalOperatingMode.UnknownUser.Id;
            model.JustMode = true;

            if (_notificationService != null)
                _notificationService.SendHeartbeat(model);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(SendData, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(499));
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
    }
}