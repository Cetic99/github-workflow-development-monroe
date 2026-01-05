using System;
using System.Reflection;

namespace CashVault.Domain.Aggregates.DeviceAggregate;

public class TerminalServerAction
{
    public Guid Uuid { get; set; }
    public string? ExternalId { get; set; }
    public MethodInfo Method { get; set; }
    public object?[]? Parameters { get; set; }
    public bool IsSuccess { get; set; } = false;
    public bool IsCompleted { get; set; } = false;
    public string? ResponseMessage { get; set; }
}
