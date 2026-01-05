using CashVault.Domain.Aggregates.DeviceAggregate;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class TerminalUserWidgetDto
    {
        public Guid Uuid { get; set; }
        public string Code { get; set; } = string.Empty;
        public int DisplaySequence { get; set; } = 9999;
        public string Size { get; set; } = UserWidgetSize.DefaultWidgetSize.Code;
    }
}
