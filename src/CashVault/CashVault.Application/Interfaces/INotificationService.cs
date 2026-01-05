using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Events;
using CashVault.Domain.Aggregates.OperatorAggregate;
using CashVault.Domain.Common;
using CashVault.Domain.ValueObjects;

namespace CashVault.Application.Interfaces;

/// <summary>
/// Interface abstracting sending notification to clients from command and event handlers
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Notify all clients that a bill accepting is in progress
    /// </summary>
    Task BillAccepting();

    /// <summary>
    /// Notify all clients that a bill has been accepted
    /// </summary>
    /// <param name="amount">The amount of the bill</param>
    /// <param name="currency">The currency of the bill</param>
    Task BillAccepted(decimal amount, Currency currency);

    /// <summary>
    /// Notify all clients that a bill has been rejected
    /// </summary>
    Task BillRejected();

    /// <summary>
    /// Notify all clients that a ticket has been accepted
    /// </summary>
    /// <param name="amount">The amount of the ticket</param>
    /// <param name="currency">The currency of the ticket</param>
    Task TicketAccepted(decimal amount, Currency currency);

    /// <summary>
    /// Notify all clients that a ticket has been rejected
    /// </summary>
    Task TicketRejected();

    /// <summary>
    /// Notify all clients that a bill has been accepted
    /// </summary>
    /// <param name="amount">The amount of the coin</param>
    /// <param name="currency">The currency of the coin</param>
    Task CoinAccepted(decimal amount, Currency currency);


    /// <summary>
    /// Notify all clients that a coin has been rejected
    /// </summary>
    Task CoinRejected();

    /// <summary>
    /// Notify all clients that an operation has been executed (device enabled/disabled, activated/deactivated, etc.)
    /// </summary>
    /// <param name="operationUuid">Uuid of the operation</param>
    /// <param name="message">Message</param>
    Task OperationExecuted(Guid operationUuid, string message, bool isSuccess);

    /// <summary>
    /// Notify all clients that the authentication is successfull
    /// </summary>
    /// <param name="@operator">Operator that authenticated</param>
    /// <returns></returns>
    Task AuthenticationSuccessfull(Operator @operator);

    /// <summary>
    /// Notify all clients that the authentication failed
    /// </summary>
    /// <returns></returns>
    Task AuthenicationFailed();

    // <summary>
    /// Sends an error message to all clients connected to the MoneyServicesHub.
    /// </summary>
    /// <param name="message">The error message to be sent.</param>
    Task MoneyStatusError(string message);

    /// <summary>
    /// Send heartbeat to all clients
    /// </summary>
    /// <returns></returns>
    Task SendHeartbeat(HeartbeatModel model);

    /// <summary>
    /// Send heartbeat to all clients
    /// </summary>
    /// <returns></returns>
    Task MessageUpdated(string langCode, string key, string value);

    #region Device
    // <summary>
    /// Sends an error message to all clients connected to the DeviceEventsHub.
    /// </summary>
    /// <param name="message">DeviceFailEvent with information about deviceType, error message...</param>
    Task DeviceError(DeviceErrorOccuredEvent deviceError);

    /// <summary>
    /// Sends a device warning event to the DeviceEventsHub.
    /// </summary>
    Task DeviceWarning(DeviceWarningRaisedEvent deviceWarning);

    /// <summary>
    /// Sends a device enabled event to the DeviceEventsHub.
    /// </summary>
    Task DeviceEnabled(DeviceEnabledEvent deviceEnabled);

    /// <summary>
    /// Sends a device disabled event to the DeviceEventsHub.
    /// </summary>
    Task DeviceDisabled(DeviceDisabledEvent deviceDisabled);

    /// <summary>
    /// Sends a device activated event to the DeviceEventsHub.
    /// </summary>
    Task DeviceActivated(DeviceActivatedEvent deviceActivated);

    /// <summary>
    /// Sends a device deactivated event to the DeviceEventsHub.
    /// </summary>
    Task DeviceDeactivated(DeviceDeactivatedEvent deviceDeactivated);

    /// <summary>
    /// Sends a device connected event to the DeviceEventsHub.
    /// </summary>
    Task DeviceConnected(DeviceConnectedEvent deviceConnected);

    /// <summary>
    /// Sends a device disconnected event to the DeviceEventsHub.
    /// </summary>
    Task DeviceDisconnected(DeviceDisconnectedEvent deviceDisconnected);

    #endregion

    #region CMS
    /// <summary>
    /// Sends info about is CMS connected or not
    /// </summary>
    /// <param name="status">Is CMS enabled and connectivity status</param>
    Task CMSConnectivityStatus(CMSConnectivityStatusModel status);

    #endregion

    #region Operator card

    Task CardReaderInitialized();

    Task CardReaderInitialzationFailed();

    Task CardScanCompleted();

    Task CardScanFailed();

    Task CardEnrolled();

    Task CardeEnrolmentFailed();

    #endregion

    #region User widgets

    /// <summary>
    /// Send updated user widgets to all clients.
    /// </summary>
    Task UserWidgetsUpdated(List<UserWidget> widgets);

    #endregion

    #region Parcel locker

    Task ParcelLockerClosed(int parcelLocker);

    #endregion
}
