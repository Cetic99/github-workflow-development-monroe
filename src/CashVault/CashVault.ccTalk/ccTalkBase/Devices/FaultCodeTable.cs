using System;
using System.Collections.Generic;
using System.Linq;

namespace CashVault.ccTalk.ccTalkBase.Devices
{
    /// <summary>
    /// Represents a single fault code with description and optional extra information
    /// </summary>
    public class FaultCode
    {
        public byte Code { get; private set; }
        public string Description { get; private set; }
        public string? OptionalExtraInfo { get; private set; }
        public byte? ExtraInfoValue { get; private set; }
        public DateTime Timestamp { get; private set; }

        public FaultCode(byte code, string description, string? optionalExtraInfo = null, byte? extraInfoValue = null)
        {
            Code = code;
            Description = description;
            OptionalExtraInfo = optionalExtraInfo;
            ExtraInfoValue = extraInfoValue;
            Timestamp = DateTime.UtcNow;
        }

        public bool IsOk => Code == 0;
        public bool HasExtraInfo => ExtraInfoValue.HasValue;

        public override string ToString()
        {
            var result = $"Fault {Code}: {Description}";
            if (HasExtraInfo && !string.IsNullOrEmpty(OptionalExtraInfo))
            {
                result += $" ({OptionalExtraInfo}: {ExtraInfoValue})";
            }
            return result;
        }
    }

    /// <summary>
    /// ccTalk fault code definitions and management according to ccTalk specification
    /// </summary>
    public static class FaultCodeTable
    {
        private static readonly Dictionary<byte, (string Description, string? OptionalExtraInfo)> _faultCodes = 
            new Dictionary<byte, (string, string?)>
            {
                { 0, ("OK (no fault detected)", null) },
                { 1, ("EEPROM checksum corrupted", null) },
                { 2, ("Fault on inductive coils", "Coil number") },
                { 3, ("Fault on credit sensor", null) },
                { 4, ("Fault on piezo sensor", null) },
                { 5, ("Fault on reflective sensor", null) },
                { 6, ("Fault on diameter sensor", null) },
                { 7, ("Fault on wake-up sensor", null) },
                { 8, ("Fault on sorter exit sensors", "Sensor number") },
                { 9, ("NVRAM checksum corrupted", null) },
                { 10, ("Coin dispensing error", null) },
                { 11, ("Low level sensor error", "Hopper or tube number") },
                { 12, ("High level sensor error", "Hopper or tube number") },
                { 13, ("Coin counting error", null) },
                { 14, ("Keypad error", "Key number") },
                { 15, ("Button error", null) },
                { 16, ("Display error", null) },
                { 17, ("Coin auditing error", null) },
                { 18, ("Fault on reject sensor", null) },
                { 19, ("Fault on coin return mechanism", null) },
                { 20, ("Fault on C.O.S. mechanism", null) },
                { 21, ("Fault on rim sensor", null) },
                { 22, ("Fault on thermistor", null) },
                { 23, ("Payout motor fault", "Hopper number") },
                { 24, ("Payout timeout", "Hopper or tube number") },
                { 25, ("Payout jammed", "Hopper or tube number") },
                { 26, ("Payout sensor fault", "Hopper or tube number") },
                { 27, ("Level sensor error", "Hopper or tube number") },
                { 28, ("Personality module not fitted", null) },
                { 29, ("Personality checksum corrupted", null) },
                { 30, ("ROM checksum mismatch", null) },
                { 31, ("Missing slave device", "Slave address") },
                { 32, ("Internal comms bad", "Slave address") },
                { 33, ("Supply voltage outside operating limits", null) },
                { 34, ("Temperature outside operating limits", null) },
                { 35, ("D.C.E. fault", "1 = coin, 2 = token") },
                { 36, ("Fault on bill validation sensor", "Sensor number") },
                { 37, ("Fault on bill transport motor", null) },
                { 38, ("Fault on stacker", null) },
                { 39, ("Bill jammed", null) },
                { 40, ("RAM test fail", null) },
                { 41, ("Fault on string sensor", null) },
                { 42, ("Accept gate failed open", null) },
                { 43, ("Accept gate failed closed", null) },
                { 44, ("Stacker missing", null) },
                { 45, ("Stacker full", null) },
                { 46, ("Flash memory erase fail", null) },
                { 47, ("Flash memory write fail", null) },
                { 48, ("Slave device not responding", "Device number") },
                { 49, ("Fault on opto sensor", "Opto number") },
                { 50, ("Battery fault", null) },
                { 51, ("Door open", null) },
                { 52, ("Microswitch fault", null) },
                { 53, ("RTC fault", null) },
                { 54, ("Firmware error", null) },
                { 55, ("Initialisation error", null) },
                { 56, ("Supply current outside operating limits", null) },
                { 57, ("Forced bootloader mode", null) },
                { 255, ("Unspecified fault code", "Further information") }
            };

        /// <summary>
        /// Creates a FaultCode instance from raw fault data
        /// </summary>
        /// <param name="faultCode">The fault code byte</param>
        /// <param name="extraInfoValue">Optional extra information value</param>
        /// <returns>FaultCode instance</returns>
        public static FaultCode CreateFaultCode(byte faultCode, byte? extraInfoValue = null)
        {
            if (_faultCodes.TryGetValue(faultCode, out var faultInfo))
            {
                return new FaultCode(faultCode, faultInfo.Description, faultInfo.OptionalExtraInfo, extraInfoValue);
            }
            else
            {
                // Unknown fault code
                return new FaultCode(faultCode, $"Unknown fault code: {faultCode}", "Raw code", extraInfoValue);
            }
        }

        /// <summary>
        /// Gets the description for a specific fault code
        /// </summary>
        /// <param name="faultCode">The fault code</param>
        /// <returns>Description string</returns>
        public static string GetDescription(byte faultCode)
        {
            return _faultCodes.TryGetValue(faultCode, out var faultInfo) 
                ? faultInfo.Description 
                : $"Unknown fault code: {faultCode}";
        }

        /// <summary>
        /// Checks if a fault code expects extra information
        /// </summary>
        /// <param name="faultCode">The fault code</param>
        /// <returns>True if extra info is expected</returns>
        public static bool HasOptionalExtraInfo(byte faultCode)
        {
            return _faultCodes.TryGetValue(faultCode, out var faultInfo) && 
                   !string.IsNullOrEmpty(faultInfo.OptionalExtraInfo);
        }

        /// <summary>
        /// Gets the optional extra info description for a fault code
        /// </summary>
        /// <param name="faultCode">The fault code</param>
        /// <returns>Extra info description or null</returns>
        public static string? GetOptionalExtraInfoDescription(byte faultCode)
        {
            return _faultCodes.TryGetValue(faultCode, out var faultInfo) 
                ? faultInfo.OptionalExtraInfo 
                : null;
        }

        /// <summary>
        /// Checks if the fault code indicates an OK status
        /// </summary>
        /// <param name="faultCode">The fault code</param>
        /// <returns>True if OK (code 0)</returns>
        public static bool IsOk(byte faultCode) => faultCode == 0;

        /// <summary>
        /// Gets all known fault codes
        /// </summary>
        /// <returns>Array of all known fault code numbers</returns>
        public static byte[] GetAllKnownFaultCodes() => _faultCodes.Keys.ToArray();

        /// <summary>
        /// Categorizes fault codes by severity (basic categorization)
        /// </summary>
        /// <param name="faultCode">The fault code</param>
        /// <returns>Severity level</returns>
        public static FaultSeverity GetSeverity(byte faultCode)
        {
            return faultCode switch
            {
                0 => FaultSeverity.None,
                1 or 9 or 29 or 30 => FaultSeverity.Critical, // Checksum/ROM errors
                33 or 34 or 56 => FaultSeverity.Critical, // Power/temperature limits
                40 or 54 or 55 => FaultSeverity.Critical, // System errors
                44 or 45 => FaultSeverity.Warning, // Stacker issues
                51 => FaultSeverity.Warning, // Door open
                255 => FaultSeverity.Unknown, // Unspecified
                _ => FaultSeverity.Error // Default for other faults
            };
        }
    }

    /// <summary>
    /// Fault severity levels
    /// </summary>
    public enum FaultSeverity
    {
        None,       // No fault (code 0)
        Warning,    // Non-critical issue
        Error,      // Standard fault
        Critical,   // Critical system fault
        Unknown     // Unknown or unspecified fault
    }

    /// <summary>
    /// Buffer for storing multiple fault codes, useful for devices that can report multiple faults
    /// </summary>
    public class FaultCodeBuffer
    {
        private readonly List<FaultCode> _faults = new List<FaultCode>();
        private readonly int _maxCapacity;

        public FaultCodeBuffer(int maxCapacity = 10)
        {
            _maxCapacity = maxCapacity;
        }

        /// <summary>
        /// Adds a fault to the buffer
        /// </summary>
        /// <param name="fault">The fault code to add</param>
        public void AddFault(FaultCode fault)
        {
            _faults.Add(fault);
            
            // Remove oldest faults if buffer is full
            if (_faults.Count > _maxCapacity)
            {
                _faults.RemoveAt(0);
            }
        }

        /// <summary>
        /// Adds a fault from raw data
        /// </summary>
        /// <param name="faultCode">Fault code byte</param>
        /// <param name="extraInfoValue">Optional extra info</param>
        public void AddFault(byte faultCode, byte? extraInfoValue = null)
        {
            var fault = FaultCodeTable.CreateFaultCode(faultCode, extraInfoValue);
            AddFault(fault);
        }

        /// <summary>
        /// Gets all faults in the buffer
        /// </summary>
        /// <returns>Array of fault codes</returns>
        public FaultCode[] GetAllFaults() => _faults.ToArray();

        /// <summary>
        /// Gets active faults (excluding OK status)
        /// </summary>
        /// <returns>Array of active fault codes</returns>
        public FaultCode[] GetActiveFaults() => _faults.Where(f => !f.IsOk).ToArray();

        /// <summary>
        /// Gets the most recent fault
        /// </summary>
        /// <returns>Most recent fault or null if buffer is empty</returns>
        public FaultCode? GetLatestFault() => _faults.LastOrDefault();

        /// <summary>
        /// Checks if there are any active faults
        /// </summary>
        /// <returns>True if there are active faults</returns>
        public bool HasActiveFaults() => _faults.Any(f => !f.IsOk);

        /// <summary>
        /// Clears all faults from the buffer
        /// </summary>
        public void Clear() => _faults.Clear();

        /// <summary>
        /// Gets the count of faults in the buffer
        /// </summary>
        public int Count => _faults.Count;

        /// <summary>
        /// Gets faults by severity level
        /// </summary>
        /// <param name="severity">Severity level to filter by</param>
        /// <returns>Array of faults matching the severity</returns>
        public FaultCode[] GetFaultsBySeverity(FaultSeverity severity)
        {
            return _faults.Where(f => FaultCodeTable.GetSeverity(f.Code) == severity).ToArray();
        }
    }
}
