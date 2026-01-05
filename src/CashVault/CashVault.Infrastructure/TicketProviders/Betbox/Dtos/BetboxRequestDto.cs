using System.Text.Json.Serialization;

namespace CashVault.Infrastructure.TicketProviders.Betbox.Dtos;

internal class BetboxRequestDto
{
    [JsonPropertyName("barcode")]
    public string Barcode { get; set; } = string.Empty;
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    public BetboxRequestDto(string barcode, Guid? id)
    {
        Barcode = barcode;
        Id = id;
    }
}
