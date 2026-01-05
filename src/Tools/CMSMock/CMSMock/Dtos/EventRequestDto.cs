namespace CMSMock.Dtos;

internal class EventRequestDto
{
    public string Type { get; set; } = null!;
    public DateTime DateTime { get; set; }
    public string MachineName { get; set; } = null!;
    public Event Event { get; set; } = null!;
}


public class Event
{
    public Guid Uuid { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Message { get; set; }
    public string? Type { get; set; } = null!;
    public string? DeviceType { get; init; }
    public string? Name { get; set; }
}
