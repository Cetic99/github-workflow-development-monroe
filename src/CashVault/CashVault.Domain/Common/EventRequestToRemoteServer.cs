using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Common;

public class EventRequestToRemoteServer
{
    [JsonPropertyOrder(1)]
    public string Timestamp { get; private set; }
    [JsonPropertyOrder(2)]
    public string DeviceId { get; private set; }
    [JsonPropertyOrder(3)]
    public List<EventToSendDto> Events { get; private set; }

    public EventRequestToRemoteServer(string timestamp, string deviceId, List<BaseEvent> events)
    {
        Timestamp = timestamp;
        DeviceId = deviceId;
        Events = [];
        if (events != null && events.Count > 0)
        {
            foreach (var e in events)
            {
                Events.Add(new EventToSendDto
                {
                    Guid = e.Guid,
                    Name = e.EventName,
                    CreatedAt = e.Created,
                    Message = e.Message,
                    Type = e.Type,
                    User = new UserInfoDto()
                    {
                        Username = e?.CreatedByUser,
                        FullName = e?.CreatedByUserFullName,
                        Company = e?.CreatedByUserCompany
                    },
                    JsonMessage = e.Json,
                    DeviceType = e is DeviceEvent deviceEvent ? deviceEvent.DeviceType : null,
                });
            }
        }
    }
}

public class EventToSendDto
{
    [JsonPropertyOrder(1)]
    public Guid Guid { get; set; }

    [JsonPropertyOrder(2)]
    public string? Name { get; set; }

    [JsonPropertyOrder(3)]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyOrder(4)]
    public string? Message { get; set; }

    [JsonPropertyOrder(5)]
    public string? Type { get; set; }

    [JsonPropertyOrder(6)]
    public string? DeviceType { get; set; }

    [JsonPropertyOrder(7)]
    public string? JsonMessage { get; set; }

    [JsonPropertyOrder(8)]
    public UserInfoDto? User { get; set; }
}

public class UserInfoDto
{
    [JsonPropertyOrder(1)]
    public string? Username { get; set; }

    [JsonPropertyOrder(2)]
    public string? FullName { get; set; }

    [JsonPropertyOrder(3)]
    public string? Company { get; set; }
}