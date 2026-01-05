using System;

namespace CashVault.Domain.Common
{
    public class Entity
    {
        public int Id { get; protected set; }
        public int Version { get; private set; }
        public DateTime? Created { get; protected set; }
        public string? CreatedBy { get; protected set; }
        public DateTime? Updated { get; protected set; }
        public string? UpdatedBy { get; protected set; }
    }
}
