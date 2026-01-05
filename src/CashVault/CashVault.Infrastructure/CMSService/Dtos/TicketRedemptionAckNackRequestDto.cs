namespace CashVault.Infrastructure.CMSService.Dtos
{
    internal class TicketRedemptionAckNackRequestDto : BaseDto
    {
        public string Barcode { get; set; } = null!;

        public TicketRedemptionAckNackRequestDto(string type, DateTime dateTime, string machineName, string secretKey, string barcode) : base(type, dateTime, machineName, secretKey)
        {
            Barcode = barcode;
        }
    }
}