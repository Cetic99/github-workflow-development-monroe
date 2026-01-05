namespace CashVault.Infrastructure.CMSService.Dtos
{
    internal class TicketRedemptionRequestDto : BaseDto
    {
        public string Language { get; set; } = null!;
        public string Barcode { get; set; } = null!;

        public TicketRedemptionRequestDto(string type, DateTime dateTime, string language, string machineName, string secretKey, string barcode) : base(type, dateTime, machineName, secretKey)
        {
            Language = language;
            Barcode = barcode;
        }
    }
}
