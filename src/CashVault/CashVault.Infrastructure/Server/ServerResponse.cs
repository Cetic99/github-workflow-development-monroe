using System.Text.Json.Serialization;

namespace CashVault.Infrastructure.Server;

public class ServerResponse
{
    [JsonPropertyName("Uuid")]
    public Guid Uuid { get; set; }
    [JsonPropertyName("ExternalId")]
    public string ExternalId { get; set; } = string.Empty;
    [JsonPropertyName("CommandName")]
    public string CommandName { get; set; } = string.Empty;
    [JsonPropertyName("Params")]
    public CommandParam[] Params { get; set; } = [];
}

public class CommandParam
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("Value")]
    public object Value { get; set; } = string.Empty;
}
