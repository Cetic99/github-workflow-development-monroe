using System.Text.Json.Serialization;
using CashVault.Application.Common.Helpers;

namespace CashVault.Infrastructure.CMSService.Dtos
{
    internal class TicketPrintRequestDto : BaseDto
    {
        public string Language { get; set; } = null!;
        [JsonConverter(typeof(AmountInCentsJsonConverter))]
        public decimal Amount { get; set; }

        public TicketPrintRequestDto(string type, DateTime dateTime, string language, string machineName, string secretKey, decimal amount) : base(type, dateTime, machineName, secretKey)
        {
            Language = language;
            Amount = amount;
        }
    }


}
