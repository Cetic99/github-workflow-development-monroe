namespace CashVault.Infrastructure.CMSService.Dtos
{
    internal class TicketPrintCompleteFailResponseDto : BaseDto
    {
        public string Barcode { get; set; }
        public int ResponseCode { get; set; }

        public TicketPrintCompleteFailResponseDto(string type, DateTime dateTime, string machineName, string secretKey, string barcode, int responseCode) : base(type, dateTime, machineName, secretKey)
        {
            Barcode = barcode;
            ResponseCode = responseCode;
        }
    }
}
