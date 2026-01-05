using CashVault.BillDispenserDriver.JCM.F53.Messages.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.ResponseMessages
{
    internal class DeviceInitializationResponseMessage : EnhancedResponseMessage
    {
        public List<SensorLevelInformation> SensorLevel { get; init; }

        public DeviceInitializationResponseMessage(byte[] data) : base(data)
        {
            //Data length check
            if (data.Length != 107)
            {
                throw new ArgumentException("Invalid data length!");
            }

            //DH1 check
            if (data[1] != 0x02)
            {
                throw new ArgumentException("Invalid DH1");
            }
            DH1 = data[1];

            //DH3 check
            if (data[4] != 0x00 || data[5] != 0x64)
            {
                throw new ArgumentException("Invalid DH3");
            }
            DH3 = [data[4], data[5]];

            SensorLevel = new List<SensorLevelInformation>();
            ParseSensorLevelInformation(data.Skip(90).Take(16).ToArray());

        }

        public override bool IsValidMessage()
        {
            return true;
        }

        private void ParseSensorLevelInformation(byte[] sensorLevelInfo)
        {
            for (int i = 0; i < 6; i++) 
            {
                if (sensorLevelInfo[i] != 0x00)
                {
                    SensorLevel.Add(new SensorLevelInformation
                    {
                        SensorName = $"FDLS{i}",
                        SensorLevelValue = sensorLevelInfo[i],
                        SensorLevelValueNormal = CheckIfNormalLevelValue(sensorLevelInfo[i]),
                        MaintenanceNecessary = CheckIfMaintenanceIsNecessary(sensorLevelInfo[i]),
                        ReplacementNecessary = CheckIfReplacementIsNecessary(sensorLevelInfo[i])
                    });
                }
            }
            if (sensorLevelInfo[6] != 0x00)
            {
                SensorLevel.Add(new SensorLevelInformation
                {
                    SensorName = "DFSS",
                    SensorLevelValue = sensorLevelInfo[6],
                    SensorLevelValueNormal = CheckIfNormalLevelValue(sensorLevelInfo[6]),
                    MaintenanceNecessary = CheckIfMaintenanceIsNecessary(sensorLevelInfo[6]),
                    ReplacementNecessary = CheckIfReplacementIsNecessary(sensorLevelInfo[6])
                });
            }
            if (sensorLevelInfo[7] != 0x00)
            {
                SensorLevel.Add(new SensorLevelInformation
                {
                    SensorName = "REJS",
                    SensorLevelValue = sensorLevelInfo[7],
                    SensorLevelValueNormal = CheckIfNormalLevelValue(sensorLevelInfo[7]),
                    MaintenanceNecessary = CheckIfMaintenanceIsNecessary(sensorLevelInfo[7]),
                    ReplacementNecessary = CheckIfReplacementIsNecessary(sensorLevelInfo[7])
                });
            }
            if (sensorLevelInfo[8] != 0x00)
            {
                SensorLevel.Add(new SensorLevelInformation
                {
                    SensorName = "BPSF",
                    SensorLevelValue = sensorLevelInfo[8],
                    SensorLevelValueNormal = CheckIfNormalLevelValue(sensorLevelInfo[8]),
                    MaintenanceNecessary = CheckIfMaintenanceIsNecessary(sensorLevelInfo[8]),
                    ReplacementNecessary = CheckIfReplacementIsNecessary(sensorLevelInfo[8])
                });
            }
            if (sensorLevelInfo[14] != 0x00)
            {
                SensorLevel.Add(new SensorLevelInformation
                {
                    SensorName = "BPSF",
                    SensorLevelValue = sensorLevelInfo[14],
                    SensorLevelValueNormal = CheckIfNormalLevelValue(sensorLevelInfo[14]),
                    MaintenanceNecessary = CheckIfMaintenanceIsNecessary(sensorLevelInfo[14]),
                    ReplacementNecessary = CheckIfReplacementIsNecessary(sensorLevelInfo[14])
                });
            }
        }

        private Func<int, bool> CheckIfNormalLevelValue = value => value > 0 && value < 9;
        private Func<int, bool> CheckIfMaintenanceIsNecessary = value => value > 8 && value < 12;
        private Func<int, bool> CheckIfReplacementIsNecessary = value => value > 12 && value < 15;

    }
}
