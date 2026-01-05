using System;
using CashVault.Domain.Aggregates.OperatorAggregate;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

/// <summary>
/// Represents a card reader interface.
/// </summary>
public interface IUserCardReader : IBasicHardwareDevice
{
    /// <summary>
    /// Event raised when a card is successfully authenticated.
    /// </summary>
    event EventHandler<(string, Guid)> CardAuthenticated;

    /// <summary>
    /// Event raised when card authentication fails.
    /// </summary>
    event EventHandler<string> CardAuthenticationFailed;

    /// <summary>
    /// Event raised when a card is successfully enrolled.
    /// </summary>
    event EventHandler<string> CardEnrolled;

    /// <summary>
    /// Event raised when card enrollment fails.
    /// </summary>
    event EventHandler<string> CardEnrollmentFailed;

    /// <summary>
    /// Enrolls a card for authentication.
    /// </summary>
    /// <param name="card">The card to be enrolled.</param>
    /// <returns>True if the card enrollment is successful, otherwise false.</returns>
    bool EnrollCard(IdentificationCard card);
}
