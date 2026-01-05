using CashVault.DeviceDriver.Common;

namespace CashVault.BillDispenserDriver.JCM.F53.Interfaces
{
    /// <summary>
    /// Represents a bill dispenser message.
    /// </summary>
    internal interface IBillDispenserMessage : ISerialPortMessage
    {
        /// <summary>
        /// Checks if message has enhanced frame
        /// </summary>
        /// <returns>True if the message frame is enhanced, otherwise false.</returns>
        bool IsEnhanced();
    }
}
