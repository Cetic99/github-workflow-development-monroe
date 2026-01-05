
using System.Collections.Generic;
using System.Text.Json.Serialization;
using CashVault.Domain.Aggregates.DeviceAggregate;

namespace CashVault.Domain.Common
{
    public class HeartbeatModel
    {
        [JsonPropertyName("appVersion")]
        public string AppVersion { get; set; } = string.Empty;

        [JsonPropertyName("justMode")]
        public bool JustMode { get; set; }

        [JsonPropertyName("mode")]
        public int Mode { get; set; }

        [JsonPropertyName("devices")]
        public List<DeviceStateModel> Devices { get; set; }

        public HeartbeatModel()
        {
            JustMode = false;
            Mode = TerminalOperatingMode.UnknownUser.Id;
            Devices = new List<DeviceStateModel>();
        }
    }

    public class DeviceStateModel
    {
        [JsonPropertyName("type")]
        public string Type { get; init; } = null!;

        [JsonPropertyName("name")]
        public string Name { get; init; } = null!;

        [JsonPropertyName("status")]
        public string Status { get; init; } = null!;

        [JsonPropertyName("isEnabled")]
        public bool IsEnabled { get; init; } = false;

        [JsonPropertyName("isActive")]
        public bool IsActive { get; init; } = false;

        [JsonPropertyName("isConnected")]
        public bool IsConnected { get; init; } = false;

        [JsonPropertyName("warning")]
        public string Warning { get; init; } = null!;

        [JsonPropertyName("error")]
        public string Error { get; init; } = null!;
        [JsonPropertyName("commandInProgress")]
        public bool CommandInProgress { get; set; } = false;
    }
}
