using CashVault.Domain.Common;
using CashVault.Domain.Common.Events;

namespace CashVault.Infrastructure.CMSService.Dtos;

internal class EventRequestDto : BaseDto
{
    public Event Event { get; set; } = null!;

    public EventRequestDto(string type, DateTime dateTime, string machineName, string secretKey, BaseEvent @event) : base(type, dateTime, machineName, secretKey)
    {
        Event = new Event()
        {
            Uuid = @event.Guid,
            Timestamp = @event.Created,
            Message = @event.Message,
            Type = @event.Type,
            Name = @event.EventName,
            User = new UserInfoDto
            {
                Username = @event.CreatedByUser,
                FullName = @event.CreatedByUserFullName,
                Company = @event.CreatedByUserCompany
            },
            DeviceType = @event is DeviceEvent deviceEvent ? deviceEvent?.DeviceType : null
        };
    }
}

public class Event
{
    public Guid Uuid { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Message { get; set; }
    public string? Type { get; set; } = null!;
    public string? DeviceType { get; init; }
    public string? Name { get; set; }
    public UserInfoDto? User { get; set; }
}
