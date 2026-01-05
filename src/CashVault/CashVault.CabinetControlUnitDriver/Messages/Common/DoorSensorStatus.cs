using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.CabinetControlUnitDriver.Messages.Common
{
    internal class DoorSensorStatus
    {
        public int SensorId { get; init; }
        public bool Opened { get; init; }
    }
}
