namespace CashVault.Infrastructure.CMSService.Dtos
{
    internal class TransactionResponseDto : BaseDto
    {
        public int ResponseCode { get; set; }

        public TransactionResponseDto(string type, DateTime dateTime, string machineName, string secretKey, int responseCode)
            : base(type, dateTime, machineName, secretKey)
        {
            ResponseCode = responseCode;
        }
    }
}
