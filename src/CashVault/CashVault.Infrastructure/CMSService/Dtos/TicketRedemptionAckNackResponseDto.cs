namespace CashVault.Infrastructure.CMSService.Dtos
{
    internal class TicketRedemptionAckNackResponseDto : BaseDto
    {
        public string Barcode { get; set; } = null!;
        public int ResponseCode { get; set; }

        public TicketRedemptionAckNackResponseDto(string type, DateTime dateTime, string machineName, string secretKey, string barcode, int responseCode) : base(type, dateTime, machineName, secretKey)
        {
            Barcode = barcode;
            ResponseCode = responseCode;
        }
    }
}
