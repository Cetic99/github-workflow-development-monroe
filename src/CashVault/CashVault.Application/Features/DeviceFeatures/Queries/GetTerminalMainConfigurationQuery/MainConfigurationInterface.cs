using CashVault.Application.Common.Models;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class MainConfigurationInterface
    {
        public string Value { get; set; }
        public List<SelectListItem> Options { get; set; }

        public MainConfigurationInterface()
        {
            Options = new List<SelectListItem>();
        }
    }
}
