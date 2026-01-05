using System.Text.Json.Serialization;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Infrastructure.Server.Dtos;

namespace CashVault.Infrastructure.Server;

internal class HeartbeatPayloadModel
{
    [JsonPropertyOrder(1)]
    public string TerminalStatus { get; set; } = null!;

    [JsonPropertyOrder(2)]
    public List<TerminalType> TerminalTypes { get; set; } = null!;
    [JsonPropertyOrder(3)]
    public TerminalOperatingMode OperatingMode { get; set; } = null!;
    [JsonPropertyOrder(4)]
    public string CurrentUser { get; set; } = null!;
    [JsonPropertyOrder(5)]
    public List<DeviceStatus> PeripheralsStatuses { get; set; } = [];
    [JsonPropertyOrder(6)]
    public MoneyStatus MoneyStatus { get; set; } = null!;
    [JsonPropertyOrder(7)]
    public string Timestamp { get; set; } = null!;
    [JsonPropertyOrder(8)]
    public string SoftwareVersion { get; set; } = null!;

    [JsonPropertyOrder(9)]
    public ServerActionResponseDto? ActionResponse { get; set; }
    [JsonPropertyOrder(10)]
    public string DeviceId { get; set; }
}