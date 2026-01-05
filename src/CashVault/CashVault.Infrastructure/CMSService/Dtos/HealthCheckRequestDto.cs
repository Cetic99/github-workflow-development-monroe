namespace CashVault.Infrastructure.CMSService.Dtos;

internal class HealthCheckRequestDto : BaseDto
{
    public HealthCheckRequestDto(DateTime dateTime, string machineName, string secretKey)
        : base(CMSCommands.HealthCheck.Code, dateTime, machineName, secretKey)
    {
    }
}
