using CashVault.Application.Common.Models;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class FlashConfigurationDto
    {
        public int ThemeToShow { get; set; }
        public List<SelectListItem> ThemeOptions { get; set; }

        public string TopMessage { get; set; }

        public FlashConfigurationDto()
        {
            ThemeOptions = new List<SelectListItem>();

            ThemeToShow = -1;
            ThemeOptions.Add(new SelectListItem()
            {
                Name = "None",
                Value = "---"
            });
        }
    }
}