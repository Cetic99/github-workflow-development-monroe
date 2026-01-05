using System.Security.Claims;
using System.Text.Json;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Events;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.OperatorAggregate;
using CashVault.Domain.Common;
using CashVault.Domain.ValueObjects;
using CashVault.Infrastructure.Configuration;
using CashVault.WebAPI.Common;
using CashVault.WebAPI.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace CashVault.WebAPI.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<MoneyServicesHub, IMoneyServicesHub> _moneyServicesHub;
        private readonly IHubContext<DeviceEventsHub, IDeviceEventsHub> _deviceHub;
        private readonly IHubContext<HeartbeatHub, IHeartbeatHub> _heartbeatHub;
        private readonly ITerminal _terminal;
        private readonly JWTSettings _jwtSettings;

        public NotificationService(
            IHubContext<MoneyServicesHub, IMoneyServicesHub> hubContext,
            IHubContext<DeviceEventsHub, IDeviceEventsHub> deviceHub,
            IHubContext<HeartbeatHub, IHeartbeatHub> heartbeatHub,
            ITerminal terminal,
            IOptions<JWTSettings> jwtOptions)
        {
            _moneyServicesHub = hubContext;
            _deviceHub = deviceHub;
            _heartbeatHub = heartbeatHub;
            _terminal = terminal;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task MoneyStatusError(string message)
        {
            await _moneyServicesHub.Clients.All.SendMessage("ErrorMessage", message);
        }

        public async Task BillAccepted(decimal amount, Currency currency)
        {
            string payload = JsonSerializer.Serialize(new
            {
                amount,
                currency
            });

            await _moneyServicesHub.Clients.All.SendMessage("BillAccepted", payload);
        }

        public async Task BillRejected()
        {
            await _moneyServicesHub.Clients.All.SendMessage("BillRejected", JsonSerializer.Serialize(new { }));
        }

        public async Task BillAccepting()
        {
            await _moneyServicesHub.Clients.All.SendMessage("BillAccepting", JsonSerializer.Serialize(new { }));
        }

        public async Task TicketAccepted(decimal amount, Currency currency)
        {
            string payload = JsonSerializer.Serialize(new
            {
                amount,
                currency
            });

            await _moneyServicesHub.Clients.All.SendMessage("TicketAccepted", payload);
        }

        public async Task TicketRejected()
        {
            await _moneyServicesHub.Clients.All.SendMessage("TicketRejected", JsonSerializer.Serialize(new { }));
        }

        public async Task CoinAccepted(decimal amount, Currency currency)
        {
            string payload = JsonSerializer.Serialize(new
            {
                amount,
                currency
            });

            await _moneyServicesHub.Clients.All.SendMessage("CoinAccepted", payload);
        }

        public async Task CoinRejected()
        {
            await _moneyServicesHub.Clients.All.SendMessage("CoinRejected", JsonSerializer.Serialize(new { }));
        }

        public async Task OperationExecuted(Guid operationUuid, string message, bool isSuccess)
        {
            string payload = JsonSerializer.Serialize(new
            {
                operationUuid,
                message,
                isSuccess
            });
            await _moneyServicesHub.Clients.All.SendMessage("OperationExecuted", payload);
        }

        #region Authentication

        public async Task AuthenticationSuccessfull(Operator @operator)
        {
            var secret = _jwtSettings.Secret;
            var accessValidity = _jwtSettings.AccessTokenValidityInSeconds;
            var refreshValidity = _jwtSettings.RefreshTokenValidityInSeconds;

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, @operator.Id.ToString()),
                new(ClaimTypes.Name, @operator.Username),
                new(CustomClaimTypes.UserFullName, @operator.GetFullName()),
                new(CustomClaimTypes.UserCompany, @operator.Company ?? string.Empty)
            };

            if (@operator.Permissions.Any())
                claims.Add
                    (new Claim(CustomClaimTypes.Permisssions,
                     JsonSerializer.Serialize(@operator.Permissions.Select(x => x.Code).ToList())));

            var accessToken =
                JwtTokenHelper.GenerateJwtToken
                    (JwtTokenType.Access, secret, DateTime.UtcNow.AddSeconds(accessValidity), claims);

            var refreshToken =
                JwtTokenHelper.GenerateJwtToken
                    (JwtTokenType.Refresh, secret, DateTime.UtcNow.AddSeconds(refreshValidity), claims);

            string payload = JsonSerializer.Serialize(new
            {
                accessToken,
                refreshToken
            });

            _terminal.SetOperatingMode(TerminalOperatingMode.Operator);
            await _moneyServicesHub.Clients.All.SendMessage("AuthenticationSuccessfull", payload);
        }

        public async Task AuthenicationFailed()
        {
            _terminal.SetOperatingMode(TerminalOperatingMode.UnknownUser);
            await _moneyServicesHub.Clients.All.SendMessage("AuthenticationFailed", String.Empty);
        }

        #endregion

        #region Device
        public async Task DeviceError(DeviceErrorOccuredEvent deviceError) => await SendDeviceEventAsync("DeviceError", deviceError);

        public async Task DeviceWarning(DeviceWarningRaisedEvent deviceWarning) => await SendDeviceEventAsync("DeviceWarning", deviceWarning);

        public async Task DeviceEnabled(DeviceEnabledEvent deviceEnabled) => await SendDeviceEventAsync("DeviceEnabled", deviceEnabled);

        public async Task DeviceDisabled(DeviceDisabledEvent deviceDisabled) => await SendDeviceEventAsync("DeviceDisabled", deviceDisabled);

        public async Task DeviceActivated(DeviceActivatedEvent deviceActivated) => await SendDeviceEventAsync("DeviceActivated", deviceActivated);

        public async Task DeviceDeactivated(DeviceDeactivatedEvent deviceDeactivated) => await SendDeviceEventAsync("DeviceDeactivated", deviceDeactivated);

        public async Task DeviceConnected(DeviceConnectedEvent deviceConnected) => await SendDeviceEventAsync("DeviceConnected", deviceConnected);

        public async Task DeviceDisconnected(DeviceDisconnectedEvent deviceDisconnected) => await SendDeviceEventAsync("DeviceDisconnected", deviceDisconnected);

        private async Task SendDeviceEventAsync<BaseEvent>(string eventType, BaseEvent deviceEvent)
        {
            string payload = JsonSerializer.Serialize(deviceEvent);
            await _deviceHub.Clients.All.DeviceStatus(eventType, payload);
        }
        #endregion

        #region Heartbeat

        public async Task SendHeartbeat(HeartbeatModel model)
        {
            await _heartbeatHub.Clients.All.Heartbeat(JsonSerializer.Serialize(model));
        }

        public async Task MessageUpdated(string langCode, string key, string value)
        {
            await _heartbeatHub.Clients.All.MessageUpdated(JsonSerializer.Serialize(new
            {
                languageCode = langCode,
                key,
                value
            }));
        }

        #endregion

        #region CMS 
        public async Task CMSConnectivityStatus(CMSConnectivityStatusModel status)
        {
            await _heartbeatHub.Clients.All.CMSConnectivityStatus(JsonSerializer.Serialize(status));
        }
        #endregion

        #region Operator card

        public async Task CardReaderInitialized() => await SendDeviceMessageAsync("CardReaderInitialized", new { });

        public async Task CardScanCompleted() => await SendDeviceMessageAsync("CardScanCompleted", new { });

        public async Task CardEnrolled() => await SendDeviceMessageAsync("CardEnrolled", new { });

        public async Task CardReaderInitialzationFailed() => await SendDeviceMessageAsync("CardReaderInitialzationFailed", new { });

        public async Task CardScanFailed() => await SendDeviceMessageAsync("CardScanFailed", new { });

        public async Task CardeEnrolmentFailed() => await SendDeviceMessageAsync("CardeEnrolmentFailed", new { });

        private async Task SendDeviceMessageAsync(string eventType, object message)
        {
            string payload = JsonSerializer.Serialize(message);
            await _deviceHub.Clients.All.SendMessage(eventType, payload);
        }

        #endregion

        #region User widgets

        public async Task UserWidgetsUpdated(List<UserWidget> widgets)
        {
            await SendDeviceMessageAsync("UserWidgetsUpdated", widgets);
        }

        #endregion

        #region Parcel locker

        public async Task ParcelLockerClosed(int parcelLocker)
            => await SendDeviceMessageAsync("ParcelLockerClosed", new { parcelLocker });

        #endregion
    }
}
