namespace CashVault.Domain.Common
{
    public class EventLogType : Enumeration
    {
        public static EventLogType FailEvent = new(1, "DeviceFailEvent");
        public static EventLogType DeviceEvent = new(2, "DeviceEvent");
        public static EventLogType TransactionEvent = new(3, "TransactionEvent");
        public static EventLogType WarningEvent = new(3, "DeviceWarningEvent");

        public EventLogType(int id, string name) : base(id, name) { }
    }
}
