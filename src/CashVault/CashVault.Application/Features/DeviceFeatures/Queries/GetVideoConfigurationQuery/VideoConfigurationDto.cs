using CashVault.Application.Common.Models;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class VideoConfigurationDto
    {
        public string VideoDeviceType { get; set; }
        public List<SelectListItem> VideoDeviceTypeOptions { get; set; }
        public string VideoCaptureDevice { get; set; }
        public List<SelectListItem> VideoCaptureDeviceOptions { get; set; }
        public string VideoDeviceIp { get; set; }
        public bool VideoStreamingServer { get; set; }

        public VideoConfigurationDto()
        {
            VideoDeviceTypeOptions = new List<SelectListItem>();
            VideoCaptureDeviceOptions = new List<SelectListItem>();

            VideoDeviceType = "---";
            VideoCaptureDevice = "---";

            VideoDeviceTypeOptions.Add(new SelectListItem()
            {
                Name = "None",
                Value = "---"
            });

            VideoCaptureDeviceOptions.Add(new SelectListItem()
            {
                Name = "None",
                Value = "---"
            });
        }
    }
}