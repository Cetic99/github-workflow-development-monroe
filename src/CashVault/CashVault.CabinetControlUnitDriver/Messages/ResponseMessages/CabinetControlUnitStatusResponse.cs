using CashVault.CabinetControlUnitDriver.Messages.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CashVault.CabinetControlUnitDriver.Messages.ResponseMessages
{
    internal class CabinetControlUnitStatusResponse : DoorSensorResponse
    {
        public List<CabinetControlUnitError> Errors { get; init; } = new();
        public List<CabinetControlUnitWarning> Warnings { get; init; } = new();

        public CabinetControlUnitStatusResponse(byte[] data) : base(data)
        {
            if (data[2] != 0x63)
            {
                throw new ArgumentException("Not FW version response msg!");
            }

            CMD = data[2];
            MSG_LENGTH = data[1];
            PAYLOAD = data.Skip(3).Take(MSG_LENGTH - 5).ToArray();
            CRC = data.Skip(MSG_LENGTH - 2).Take(2).ToArray();
            ParseStatusPayload(data.Skip(3).Take(MSG_LENGTH - 5).ToArray());
        }

        private void ParseStatusPayload(byte[] payload)
        {
            foreach (var code in payload)
            {
                switch (code)
                {
                    case ErrorCodes.TEMPERATURE_SENSOR_ERROR:
                        Errors.Add(new CabinetControlUnitError
                        {
                            Code = code,
                            Description = "Temperature sensor error"
                        });
                        break;
                    case WarningCodes.HIGH_TEMPERATURE:
                        Warnings.Add(new CabinetControlUnitWarning
                        {
                            Code = code,
                            Description = "High temperature detected"
                        });
                        break;
                    case WarningCodes.VIBRATION_DETECTED:
                        Warnings.Add(new CabinetControlUnitWarning
                        {
                            Code = code,
                            Description = "Vibration detected"
                        });
                        break;
                    case ErrorCodes.CODE_ERROR:
                        Errors.Add(new CabinetControlUnitError
                        {
                            Code = code,
                            Description = "General error"
                        });
                        break;
                    case CODE_OK:
                        // Handle general success code if needed
                        break;
                    default:
                        Errors.Add(new CabinetControlUnitError
                        {
                            Code = code,
                            Description = $"Unknown error code 0x{code:X2}"
                        });
                        // Handle unknown codes if needed
                        break;
                }
            }
        }

        const byte CODE_OK = 0x00;
    }

    internal static class ErrorCodes
    {
        public const byte TEMPERATURE_SENSOR_ERROR = 0x01;
        public const byte CODE_ERROR = 0xFF; // General error code
    }

    internal static class WarningCodes
    {
        public const byte HIGH_TEMPERATURE = 0x80;
        public const byte VIBRATION_DETECTED = 0x81;
    }
}
