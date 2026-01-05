using System;

namespace CashVault.ccTalk.BillAcceptorBase
{
    /// <summary>
    /// Event arguments for device status change events
    /// </summary>
    public class DeviceStatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Previous device status
        /// </summary>
        public BillAcceptorDeviceStatus OldStatus { get; init; }

        /// <summary>
        /// New device status
        /// </summary>
        public BillAcceptorDeviceStatus NewStatus { get; init; }

        /// <summary>
        /// Timestamp when the status change was detected
        /// </summary>
        public DateTime Timestamp { get; init; }

        /// <summary>
        /// Optional reason or description for the status change
        /// </summary>
        public string? Reason { get; init; }

        public DeviceStatusChangedEventArgs(
            BillAcceptorDeviceStatus oldStatus, 
            BillAcceptorDeviceStatus newStatus, 
            string? reason = null)
        {
            OldStatus = oldStatus;
            NewStatus = newStatus;
            Timestamp = DateTime.UtcNow;
            Reason = reason;
        }
    }
}
