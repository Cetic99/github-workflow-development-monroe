namespace CashVault.Infrastructure.CMSService.Dtos
{
    internal class EventResponseDto : BaseDto
    {
        public int ResponseCode { get; set; }

        public EventResponseDto(string type, DateTime dateTime, string machineName, string secretKey, int responseCode)
            : base(type, dateTime, machineName, secretKey)
        {
            ResponseCode = responseCode;
        }
    }
}
