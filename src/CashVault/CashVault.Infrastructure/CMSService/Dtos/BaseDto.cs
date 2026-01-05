using System.Text.Json.Serialization;
using CashVault.Application.Common.Helpers;

namespace CashVault.Infrastructure.CMSService.Dtos;

internal class BaseDto
{
    public string Type { get; set; } = null!;

    [JsonPropertyName("date")]
    [JsonConverter(typeof(UnixTimestampMillisecondsJsonConverter))]
    public DateTime DateTime { get; set; }

    public string MachineName { get; set; } = null!;
    public string SecretKey { get; set; } = null!;

    public BaseDto(string type, DateTime dateTime, string machineName, string secretKey)
    {
        Type = type;
        DateTime = dateTime;
        MachineName = machineName;
        SecretKey = secretKey;
    }
}
