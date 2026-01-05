using System.Collections.Generic;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.OperatorAggregate.Events
{
    public class OperatorPermissionsUpdatedEvent : DeviceEvent
    {
        public OperatorPermissionsUpdatedEvent(Operator @operator, List<Permission> newPermissions)
           : base($"Permissions for operator: {@operator.GetFullName()} are updated.",
                  DeviceAggregate.DeviceType.General)
        {
            Operator = @operator;
            NewPermissions = newPermissions;
        }

        public Operator Operator { get; private set; }
        public List<Permission> NewPermissions { get; private set; }
    }
}
