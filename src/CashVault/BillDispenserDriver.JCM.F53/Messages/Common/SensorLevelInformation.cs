using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.Common
{
    internal class SensorLevelInformation
    {
        public string SensorName { get; init; }
        public int SensorLevelValue { get; init; }
        public bool SensorLevelValueNormal { get; init; }
        public bool MaintenanceNecessary { get; init; }
        public bool ReplacementNecessary { get; init; }
    }
}
