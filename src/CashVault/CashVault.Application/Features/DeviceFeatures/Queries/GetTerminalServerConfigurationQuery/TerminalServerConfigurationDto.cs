using CashVault.Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class TerminalServerConfigurationDto
    {
        public string Url { get; set; }
        public string DeviceId { get; set; }
        public bool IsEnabled { get; set; }
        public int MinimalLogLevel { get; set; }

        public List<SelectListItem> LogLevels { get; set; }

        public TerminalServerConfigurationDto()
        {
            LogLevels = [];

            foreach (var logLevel in Enum.GetValues(typeof(LogLevel)))
            {
                LogLevels.Add(new SelectListItem()
                {
                    Name = logLevel.ToString(),
                    Value = ((int)logLevel).ToString()
                });
            }
        }
    }
}