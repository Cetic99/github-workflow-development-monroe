namespace CashVault.Infrastructure.CMSService.Dtos
{
    internal class TicketPrintCompleteFailRequestDto : BaseDto
    {
        public string Barcode { get; set; }

        public TicketPrintCompleteFailRequestDto(string type, DateTime dateTime, string machineName, string secretKey, string barcode) : base(type, dateTime, machineName, secretKey)
        {
            Barcode = barcode;
        }
    }
}
