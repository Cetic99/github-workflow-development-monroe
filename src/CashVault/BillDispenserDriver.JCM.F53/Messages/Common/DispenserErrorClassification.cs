using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.Common
{
    internal class DispenserErrorClassification
    {
        public string? Code { get; init; }
        public string? Description { get; init; }
        public bool IsCritical { get; init; }
        public bool MaintenanceNeeded { get; init; }
    }
}
