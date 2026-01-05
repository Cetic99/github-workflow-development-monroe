using System.IO.Ports;

namespace CashVault.DeviceDriver.Common;

public class SerialPortConfiguration
{
    public string? PortName { get; set; } = "COM1";
    public int BaudRate { get; set; } = 9600;
    public int DataBits { get; set; } = 8;
    public StopBits StopBits { get; set; } = StopBits.One;
    public Parity Parity { get; set; } = Parity.Even;
    public int ReadTimeout { get; set; } = 0;
    public int WriteTimeout { get; set; } = 0;

}
