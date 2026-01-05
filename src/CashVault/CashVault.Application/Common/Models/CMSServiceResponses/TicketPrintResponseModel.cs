using System.Text.Json.Serialization;
using CashVault.Application.Common.Helpers;

namespace CashVault.Application.Common.Models.CMSResponses
{
    public class TicketPrintResponseModel
    {
        public bool IsSuccessful { get; set; }

        public string Type { get; set; } = null!;
        [JsonPropertyName("date")]
        [JsonConverter(typeof(UnixTimestampMillisecondsJsonConverter))]
        public DateTime DateTime { get; set; }
        public string Language { get; set; } = null!;
        public string MachineName { get; set; } = null!;
        public int ResponseCode { get; set; } = 9999;
        public string Barcode { get; set; } = null!;
        [JsonConverter(typeof(AmountInCentsJsonConverter))]
        public decimal Amount { get; set; }
        public int Validity { get; set; }
        public string AmountText { get; set; } = string.Empty;
        public string AmountInWords { get; set; } = string.Empty;
        [JsonConverter(typeof(StringToDateTimeJsonConverter))]
        public DateTime? DatePrint { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
    }
}
