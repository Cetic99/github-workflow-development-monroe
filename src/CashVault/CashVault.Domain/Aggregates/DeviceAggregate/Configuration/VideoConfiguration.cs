namespace CashVault.Domain.Aggregates.DeviceAggregate.Configuration;

public class VideoConfiguration
{
    public string VideoDeviceType { get; set; }
    public string VideoCaptureDevice { get; set; }
    public string VideoDeviceIp { get; set; }
    public bool VideoStreamingServer { get; set; }


    public VideoConfiguration()
    {
        VideoDeviceType = "---";
        VideoCaptureDevice = "---";
        VideoDeviceIp = "";
        VideoStreamingServer = false;
    }
}