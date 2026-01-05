namespace CashVault.DeviceDriver.Common;

/// <summary>
/// Represents a serial port driver interface.
/// </summary>
public interface ISerialPortDriver : IDisposable
{
    /// <summary>
    /// Event that is raised when the device is disconnected.
    /// </summary>
    event EventHandler DeviceDisconnected;

    /// <summary>
    /// Opens the connection with the specified serial port.
    /// </summary>
    /// <param name="portConfiguration">The configuration of the serial port to open.</param>
    /// <returns>True if the connection is successfully opened, otherwise false.</returns>
    Task<bool> OpenConnectionAsync(SerialPortConfiguration portConfiguration);

    /// <summary>
    /// Closes the connection.
    /// </summary>
    /// <returns>True if the connection is successfully closed, otherwise false.</returns>
    Task<bool> CloseConnectionAsync();

    /// <summary>
    /// Reopens the connection with the specified serial port.
    /// </summary>
    /// <param name="newPortConfiguration">The new configuration of the serial port to open. If null, the previous port will be used.</param>
    /// <returns>True if the connection is successfully reopened, otherwise false.</returns>
    Task<bool> ReopenConnectionAsync(SerialPortConfiguration? newPortConfiguration = null);

    /// <summary>
    /// Sends a message through the serial port and waits for response.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="waitingForResponseTimeoutMilliseconds">The timeout for waiting for a response.</param>
    /// <returns>The response message if received, otherwise null.</returns>
    Task<ISerialPortMessage?> SendAndReceiveMessageAsync(ISerialPortMessage message, long waitingForResponseTimeoutMilliseconds = 5000);

    /// <summary>
    /// Sends a message through the serial port, without waiting for response.
    /// </summary>
    /// <param name="message">The message to send</param>
    /// <returns></returns>
    Task SendMessageAsync(ISerialPortMessage message);

    /// <summary>
    /// Performs a device readiness check, by issuing a command to the device and waiting for a response.
    /// </summary>
    /// <param name="maximumRetryCount">The maximum number of retries for the device readiness check.</param>
    /// <param name="maximumWaitingTime">The maximum waiting time for the device readiness check. Expressed in milliseconds</param>
    void DeviceReadinessCheck(int maximumRetryCount = 5, long maximumWaitingTime = 10000);
}
