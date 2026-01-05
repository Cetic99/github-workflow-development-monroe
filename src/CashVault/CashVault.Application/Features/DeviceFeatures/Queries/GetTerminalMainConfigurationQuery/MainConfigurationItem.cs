using CashVault.Application.Common.Models;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class MainConfigurationItem
    {
        public string? Name { get; set; }
        public string? Label { get; set; }
        public string Value { get; set; }
        public List<SelectListItem> Options { get; set; }
        public MainConfigurationInterface Interface { get; set; }

        public MainConfigurationItem()
        {
            Options = new List<SelectListItem>();
        }
    }
}
