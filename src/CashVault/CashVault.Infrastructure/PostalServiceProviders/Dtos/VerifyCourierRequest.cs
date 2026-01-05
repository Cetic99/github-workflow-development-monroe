using System.Text.Json.Serialization;

namespace CashVault.Infrastructure.PostalServiceProviders.Dtos;

public class VerifyCourierRequest
{
    [JsonPropertyName("postalService")]
    public string PostalService { get; set; } = string.Empty;

    [JsonPropertyName("barcode")]
    public string Barcode { get; set; } = string.Empty;
}
