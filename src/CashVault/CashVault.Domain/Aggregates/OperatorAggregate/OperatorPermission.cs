using System;
using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.OperatorAggregate
{
    public class OperatorPermission : Entity
    {
        public Guid Guid { get; protected set; }
        public int OperatorId { get; private set; }
        public Operator Operator { get; private set; }
        private int permissionId { get; set; }
        public Permission Permission { get; private set; }

        private OperatorPermission() { }

        public OperatorPermission(Operator ooperator, Permission permission)
        {
            Operator = ooperator;
            permissionId = permission.Id;
            Guid = Guid.NewGuid();
        }
    }
}