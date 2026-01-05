using CashVault.Application.Common.Helpers;
using System.Text.Json.Serialization;

namespace CashVault.Application.Common.Models.CMSResponses
{
    public class TicketRedemptionResponseModel
    {
        public bool IsSuccessful { get; set; }

        public string Type { get; set; } = null!;
        [JsonPropertyName("date")]
        [JsonConverter(typeof(UnixTimestampMillisecondsJsonConverter))]
        public DateTime DateTime { get; set; }
        public string Language { get; set; } = null!;
        public string MachineName { get; set; } = null!;
        public string Barcode { get; set; } = null!;
        public int ResponseCode { get; set; }
        [JsonConverter(typeof(AmountInCentsJsonConverter))]
        public decimal Amount { get; set; }
        [JsonConverter(typeof(AmountInCentsJsonConverter))]
        public decimal AmountWithTaxes { get; set; }
        public string? Taxes { get; set; }
        public string? Reason { get; set; }
    }
}
