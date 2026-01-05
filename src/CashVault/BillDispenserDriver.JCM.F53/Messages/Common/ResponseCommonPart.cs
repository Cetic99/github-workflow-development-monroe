using System.Collections;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.Common
{
    internal class ResponseCommonPart
    {
        public List<DispenserError> Errors { get; init; } = new();
        public List<DispenserErrorClassification> ErrorClassification { get; init; } = new();
        public List<DispenserErrorDetails> ErrorDetails { get; init; } = new();
        public List<BillCassetteStatus> BillCassetteStatuses { get; init; } = new();
        public List<SensorStatus> SensorStatuses { get; init; } = new();


        byte[] ErrorCode;
        /// <summary>
        /// Note: There is no detailed information in datasheet
        /// </summary>
        public byte[] Version { get; init; }

        /// <summary>
        /// POM level register shows the potentiometer output level of thickness sensor
        /// </summary>
        protected int POM { get; init; }

        /// <summary>
        /// Date information
        /// </summary>
        protected string dateInformation { get; init; }

        /// <summary>
        /// Version information
        /// 
        /// Note: There is no detailed information in datasheet (related to FW)
        /// </summary>
        protected byte[] VersionInformation { get; init; }

        /// <summary>
        /// Error address identifies FW code address where the error was detected
        /// 
        /// Note: There is no detailed information in datasheet
        /// </summary>
        protected byte[] ErrorAddress { get; init; }

        /// <summary>
        /// Reserved field
        /// </summary>
        protected int RSV2 { get; init; }

        /// <summary>
        /// Error classification register2 and Error detailed register2
        /// Note: There is no detailed information in datasheet 
        /// 
        /// CountErrorDetailedRegister2 parser is implemented.
        /// </summary>
        protected byte ErrorClassificationRegister2 { get; init; }
        protected byte ErrorDetailedRegister2 { get; init; }

        /// <summary>
        /// Sensor registers 2
        /// Note: There is no detailed information in datasheet
        /// </summary>
        protected byte[] SensorRegister2 { get; init; }

        /// <summary>
        /// Reserved field
        /// </summary>
        protected int RSV3 { get; init; }


        public ResponseCommonPart(byte[] data)
        {
            if (data.Length != 84)
            {
                throw new ArgumentException("ResponseCommonPart data length must be 84 bytes");
            }

            //Error code
            ErrorCode = [data[0], data[1]];

            ParseErrorCode(ErrorCode);

            //Version
            Version = data.Skip(2).Take(4).ToArray();

            //Error registers
            ParseResponseErrorClassificationRegister(data[6]);
            ParseResponseErrorDetailedRegister(data[7]);
            ParseResponseCountErrorDetailedRegister(data[8]);

            // Cassette registers
            BillCassetteStatuses = new List<BillCassetteStatus>();
            for (int i = 1; i < 5; i++)
            {
                // The first four cassettes
                BillCassetteStatuses.Add(
                    new BillCassetteStatus(
                        // cassette number
                        i,
                        // bill length
                        BillLengthCalculation([data[19 + i], data[20 + i]]),
                        // bill thickness
                        data[27 + i],
                        // number of status changes
                        data[31 + i],
                        // status register
                        data[14 + i]
                    )
                );

                // The second four cassettes
                BillCassetteStatuses.Add(
                    new BillCassetteStatus(
                        // cassette number
                        i + 4,
                        // bill length
                        BillLengthCalculation([data[66 + i], data[67 + i]]),
                        // bill thickness
                        data[74 + i],
                        // number of status changes
                        data[78 + i],
                        // status register
                        data[62 + i]
                    )
                );
            }

            //Sensor registers SENSO1/5 and SBRG1
            ParseSensorStatusRegister(data.Skip(9).Take(6).ToArray());

            //POM register
            POM = data[19];

            //Parse date information
            dateInformation = ParseTimeDataInformations(data.Skip(36).Take(6).ToArray());

            //Version information
            VersionInformation = data.Skip(42).Take(6).ToArray();

            //Error Address
            ErrorAddress = data.Skip(48).Take(6).ToArray();

            //Reserve field 2
            byte[] reserved2 = [data[52], data[53]];
            Array.Reverse(reserved2);
            RSV2 = BitConverter.ToInt16(reserved2, 0);

            //Error registers 2
            byte errorClassificationRegister2 = data[54];
            byte errorDetailedRegister2 = data[55];
            ParseResponseCountErrorDetailedRegister2(data[56]);

            //Sensor registers 2
            SensorRegister2 = data.Skip(57).Take(6).ToArray();

            //Reserve field 3
            byte RSV3 = data[67];
        }
        //Error codes available in datasheet - table with all errors is missing in datasheet
        private void ParseErrorCode(byte[] data)
        {

            int error = data[0];

            if (error == 0x03 || error == 0x04)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Internal error",
                    IsCritical = true
                });
            }

            if (error == 0x10)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "No 1st Cassette",
                    IsCritical = true

                });
            }

            if (error == 0x11)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "1st cassette empty",
                });
            }

            if (error == 0x12)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "1st cassette: Error related to bill diagnosis",
                    IsCritical = true

                });
            }

            if (error == 0x14)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "No 5th Cassette",
                    IsCritical = true

                });
            }

            if (error == 0x15)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "5th cassette empty",
                });
            }

            if (error == 0x16)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "5th cassette: Error related to bill diagnosis",
                    IsCritical = true

                });
            }

            if (error == 0x18)
            {
                /*TODO: Add backup option. When there are no bills in case dispense from another */
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "1st Cassette pick error",
                    IsCritical = true

                });
            }

            if (error == 0x1C)
            {
                /*TODO: Add backup option. When there are no bills in case dispense from another */
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "5th Cassette pick error",
                    IsCritical = true

                });
            }

            if (error == 0x20)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "No 2nd Cassette",
                    IsCritical = true

                });
            }

            if (error == 0x21)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "2nd cassette empty",
                });
            }

            if (error == 0x22)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "2nd cassette: Error related to bill diagnosis",
                    IsCritical = true

                });
            }

            if (error == 0x24)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "No 6th Cassette",
                    IsCritical = true

                });
            }

            if (error == 0x25)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "6th cassette empty",
                });
            }

            if (error == 0x26)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "6th cassette: Error related to bill diagnosis",
                    IsCritical = true

                });
            }

            if (error == 0x28)
            {
                /*TODO: Add backup option. When there are no bills in case dispense from another */
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "2nd Cassette pick error",
                    IsCritical = true

                });
            }

            if (error == 0x2C)
            {
                /*TODO: Add backup option. When there are no bills in case dispense from another */
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "6th Cassette pick error",
                    IsCritical = true

                });
            }

            if (error == 0x30)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "No 3rd Cassette",
                    IsCritical = true

                });
            }

            if (error == 0x31)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "3rd cassette empty",
                });
            }

            if (error == 0x32)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "3rd cassette: Error related to bill diagnosis",
                    IsCritical = true

                });
            }

            if (error == 0x34)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "No 7th Cassette",
                    IsCritical = true

                });
            }

            if (error == 0x35)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "7th cassette empty",
                });
            }

            if (error == 0x36)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "7th cassette: Error related to bill diagnosis",
                    IsCritical = true

                });
            }

            if (error == 0x38)
            {
                /*TODO: Add backup option. When there are no bills in case dispense from another */
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "3rd Cassette pick error",
                    IsCritical = true

                });
            }

            if (error == 0x3C)
            {
                /*TODO: Add backup option. When there are no bills in case dispense from another */
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "7th Cassette pick error",
                    IsCritical = true

                });
            }

            if (error == 0x40)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "No 4th Cassette",
                    IsCritical = true

                });
            }

            if (error == 0x41)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "4th cassette empty",
                });
            }

            if (error == 0x42)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "8th cassette: Error related to bill diagnosis",
                    IsCritical = true

                });
            }

            if (error == 0x44)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "No 8th Cassette",
                    IsCritical = true

                });
            }

            if (error == 0x45)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "8th cassette empty",
                });
            }

            if (error == 0x46)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "6th cassette: Error related to bill diagnosis",
                    IsCritical = true

                });
            }

            if (error == 0x48)
            {
                /*TODO: Add backup option. When there are no bills in case dispense from another */
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "4th Cassette pick error",
                    IsCritical = true

                });
            }

            if (error == 0x4C)
            {
                /*TODO: Add backup option. When there are no bills in case dispense from another */
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "8th Cassette pick error",
                    IsCritical = true

                });
            }

            if (error == 0x50)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Home position error",
                    ResetNeeded = true
                });
            }

            if (error == 51)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Upper position error",
                    ResetNeeded = true
                });
            }

            if (error == 52)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "No bill in pool section",
                    ResetNeeded = true
                });
            }

            if (error == 0x70)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Bill Remained",
                    ResetNeeded = true
                });
            }

            if (error == 0x76)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Medium pulling out",
                    ResetNeeded = true
                });
            }

            if (error == 0x78 || error == 0x7A || error == 0x7B || error == 0x7C || error == 0x7D)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Bill Jam",
                    ResetNeeded = true
                });
            }

            if (error == 0xF8)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Mecha",
                    ResetNeeded = true
                });
            }

            if (error == 0x82)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Bill length error (long)",
                    ResetNeeded = true
                });
            }

            if (error == 0x83)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Bill length error (short)",
                    ResetNeeded = true
                });
            }

            if (error == 0x84)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Thickness error",
                    ResetNeeded = true
                });
            }

            if (error == 0x85)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Pick From Wrong Cassette",
                    ResetNeeded = true
                });
            }

            if (error == 0x88)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Counted Value is Wrong",
                    ResetNeeded = true
                });
            }

            if (error == 0x89)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Potentiometer error",
                    ResetNeeded = true
                });
            }

            if (error == 0xB5)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Reject Cassette Over Flow",
                    IsCritical = true
                });
            }

            if (error == 0xE0)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "RAS command undefined",
                    ResetNeeded = true
                });
            }

            if (error == 0xE1)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Parameter not registered",
                    ResetNeeded = true
                });
            }

            if (error == 0xE4)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Bill information not provided",
                    ResetNeeded = true
                });
            }

            if (error == 0xE5)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Specification error",
                    ResetNeeded = true
                });
            }

            if (error == 0xE6)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Parameter ISO code error",
                    ResetNeeded = true
                });
            }

            if (error == 0xE8)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Bill length/ thickness information error",
                    ResetNeeded = true
                });
            }

            if (error == 0xEA)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Parameter error",
                    ResetNeeded = true
                });
            }

            if (error == 0xEC)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "FS error",
                    ResetNeeded = true
                });
            }

            if (error == 0xEE)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Command format error",
                    ResetNeeded = true
                });
            }

            if (error == 0xEF)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Command execution is impossible.",
                    ResetNeeded = true
                });
            }

            if (error == 0xF1 || error == 0xF6)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Hardware error",
                    IsCritical = true
                });
            }

            if (error == 0xF6)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Log data check sum error",
                    ResetNeeded = true
                });
            }

            if (error == 0xF8)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Sensor error",
                    ResetNeeded = true
                });
            }
            if (error == 0xFD)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Power loss during dispense.",
                    IsCritical = true
                });
            }
            if (error == 0xFC)
            {
                Errors.Add(new DispenserError
                {
                    Code = error.ToString("X"),
                    Description = "Number of notes dispensed not available.",
                    IsCritical = true
                });
            }
        }

        private void ParseResponseErrorClassificationRegister(byte data)
        {
            BitArray errorBits = new BitArray(new byte[] { data });

            if (errorBits[0] == true)
            {
                ErrorClassification.Add(new DispenserErrorClassification
                {
                    Code = "EQPNR",
                    Description = "Device not ready",
                });
            }
            if (errorBits[1] == true)
            {
                ErrorClassification.Add(new DispenserErrorClassification
                {
                    Code = "JAMER",
                    Description = "Jam",
                    IsCritical = true,
                });
            }
            if (errorBits[2] == true)
            {
                ErrorClassification.Add(new DispenserErrorClassification
                {
                    Code = "CNTNC",
                    Description = "Count not complete",

                });
            }
            if (errorBits[3] == true)
            {
                ErrorClassification.Add(new DispenserErrorClassification
                {
                    Code = "HRDTR",
                    Description = "Hardware trouble",
                    IsCritical = true,
                });
            }
            if (errorBits[4] == true)
            {
                ErrorClassification.Add(new DispenserErrorClassification
                {
                    Code = "INCND",
                    Description = "Inconsistency detected",
                });
            }
            if (errorBits[5] == true)
            {
                ErrorClassification.Add(new DispenserErrorClassification
                {
                    Code = "NMASP",
                    Description = "No media at assumed position",
                });
            }
            if (errorBits[6] == true)
            {
                ErrorClassification.Add(new DispenserErrorClassification
                {
                    Code = "MDIVP",
                    Description = "The medium position abnormal",
                });
            }
            if (errorBits[7] == true)
            {
                ErrorClassification.Add(new DispenserErrorClassification
                {
                    Code = "CPERR",
                    Description = "Calling parameter error",
                });
            }
        }

        private void ParseResponseErrorDetailedRegister(byte data)
        {
            BitArray errorBits = new BitArray(new byte[] { data });

            if (errorBits[0] == true)
            {
                ErrorDetails.Add(new DispenserErrorDetails
                {
                    Code = "CCNTR",
                    Description = "The cash cassette not ready",
                });
            }
            if (errorBits[2] == true)
            {
                ErrorDetails.Add(new DispenserErrorDetails
                {
                    Code = "RBXOV",
                    Description = "Reject box overflow",
                    MaintenanceNeeded = true,
                });
            }
            if (errorBits[3] == true)
            {
                ErrorDetails.Add(new DispenserErrorDetails
                {
                    Code = "FRSRD",
                    Description = "Number of counted bills unmatch",
                });
            }
            if (errorBits[5] == true)
            {
                ErrorDetails.Add(new DispenserErrorDetails
                {
                    Code = "SNSRA",
                    Description = "Sensor alarm or medium remained",
                });
            }
            if (errorBits[6] == true)
            {
                ErrorDetails.Add(new DispenserErrorDetails
                {
                    Code = "MHDTR",
                    Description = "Main hardware trouble",
                    IsCritical = true,
                });
            }
        }

        private void ParseResponseCountErrorDetailedRegister(byte data)
        {
            BitArray errorBits = new BitArray(new byte[] { data });

            if (errorBits[2] == true)
            {
                ErrorDetails.Add(new DispenserErrorDetails
                {
                    Code = "OTRBL",
                    Description = "Drawing bills from wrong cassette",
                });
            }
            if (errorBits[3] == true)
            {
                ErrorDetails.Add(new DispenserErrorDetails
                {
                    Code = "CLSBL",
                    Description = "Bill to bill space too short",
                });
            }
            if (errorBits[4] == true)
            {
                ErrorDetails.Add(new DispenserErrorDetails
                {
                    Code = "THKBL",
                    Description = "Bill thickness error (Multi-feed)",
                });
            }
            if (errorBits[6] == true)
            {
                ErrorDetails.Add(new DispenserErrorDetails
                {
                    Code = "SHTBL",
                    Description = "Bill length short",
                });
            }
            if (errorBits[7] == true)
            {
                ErrorDetails.Add(new DispenserErrorDetails
                {
                    Code = "LNGBL",
                    Description = "Bill length long",
                });
            }
        }

        private void ParseResponseCountErrorDetailedRegister2(byte data)
        {
            BitArray errorBits = new BitArray(new byte[] { data });

            if (errorBits[0] == true)
            {
                ErrorDetails.Add(new DispenserErrorDetails
                {
                    Code = "ECST4",
                    Description = "Count abnormality occurrence cassette code",
                });
            }
            if (errorBits[1] == true)
            {
                ErrorDetails.Add(new DispenserErrorDetails
                {
                    Code = "ECST3",
                    Description = "Count abnormality occurrence cassette code",
                });
            }
            if (errorBits[2] == true)
            {
                ErrorDetails.Add(new DispenserErrorDetails
                {
                    Code = "ECST2",
                    Description = "Count abnormality occurrence cassette code",
                });
            }
            if (errorBits[3] == true)
            {
                ErrorDetails.Add(new DispenserErrorDetails
                {
                    Code = "ECST1",
                    Description = "Count abnormality occurrence cassette code",
                });
            }
        }

        private string ParseTimeDataInformations(byte[] dateInfo)
        {
            char y1 = (char)dateInfo[0];
            char y2 = (char)dateInfo[1];
            char m1 = (char)dateInfo[2];
            char m2 = (char)dateInfo[3];
            char d1 = (char)dateInfo[4];
            char d2 = (char)dateInfo[5];

            string dateInformation = $"{y1}{y2}/{m1}{m2}/{d1}{d2}";
            return dateInformation;
        }

        private int BillLengthCalculation(byte[] billLength)
        {
            int BillLength;

            int BillLengthLong = billLength[0];
            int BillLengthShort = billLength[1];

            BillLength = (BillLengthLong + BillLengthShort) / 2;

            return BillLength;
        }

        private void ParseSensorStatusRegister(byte[] sensorData)
        {
            //SENSO1
            BitArray sensorRegister1 = new BitArray(new byte[] { sensorData[0] });

            if (sensorRegister1[0] == true)
            {
                SensorStatuses.Add(new SensorStatus
                {
                    Code = "FDLS1",
                    Description = "Media exist at 1st cassette pick sensor"
                });
            }
            if (sensorRegister1[1] == true)
            {
                SensorStatuses.Add(new SensorStatus
                {
                    Code = "FDLS2",
                    Description = "Media exist at 2nd cassette pick sensor"
                });
            }
            if (sensorRegister1[2] == true)
            {
                SensorStatuses.Add(new SensorStatus
                {
                    Code = "FDLS3",
                    Description = "Media exist at 3rd cassette pick sensor"
                });
            }
            if (sensorRegister1[3] == true)
            {
                SensorStatuses.Add(new SensorStatus
                {
                    Code = "FDLS4",
                    Description = "Media exist at 4th cassette pick sensor"
                });
            }
            if (sensorRegister1[4] == true)
            {
                SensorStatuses.Add(new SensorStatus
                {
                    Code = "FDLS5",
                    Description = "Media exist at 5th cassette pick sensor"
                });
            }
            if (sensorRegister1[5] == true)
            {
                SensorStatuses.Add(new SensorStatus
                {
                    Code = "FDLS6",
                    Description = "Media exist at 6th cassette pick sensor"
                });
            }

            //SENSO2
            BitArray sensorRegister2 = new BitArray(new byte[] { sensorData[1] });

            if (sensorRegister2[0] == true)
            {
                SensorStatuses.Add(new SensorStatus
                {
                    Code = "DFSS",
                    Description = "Media exist at path sensor"
                });
            }
            if (sensorRegister2[1] == true)
            {
                SensorStatuses.Add(new SensorStatus
                {
                    Code = "BPSF",
                    Description = "Media exist at front count path sensor"
                });
            }
            if (sensorRegister2[2] == true)
            {
                SensorStatuses.Add(new SensorStatus
                {
                    Code = "REJS",
                    Description = "Media exist at reject path sensor"
                });
            }

            //SENSO3
            BitArray sensorRegister3 = new BitArray(new byte[] { sensorData[2] });

            if (sensorRegister3[4] == true)
            {
                SensorStatuses.Add(new SensorStatus
                {
                    Code = "BPSR",
                    Description = "Media exist at rear count path sensor"
                });
            }

            //SENSO4
            BitArray sensorRegister4 = new BitArray(new byte[] { sensorData[3] });

            if (sensorRegister4[7] == true)
            {
                SensorStatuses.Add(new SensorStatus
                {
                    Code = "UnitKind",
                    Description = "F53DS"
                });
            }

            //SBRG1
            BitArray sensorRegister6 = new BitArray(new byte[] { sensorData[5] });

            if (sensorRegister6[1] == true)
            {
                SensorStatuses.Add(new SensorStatus
                {
                    Code = "REJBL",
                    Description = "Set when any bill was rejected by initialization or count commands"
                });
            }
            if (sensorRegister6[2] == true)
            {
                SensorStatuses.Add(new SensorStatus
                {
                    Code = "INIBL",
                    Description = "Set when any bill was ejected by initialization command."
                });
            }
            if (sensorRegister6[3] == true)
            {
                SensorStatuses.Add(new SensorStatus
                {
                    Code = "UNNOTE",
                    Description = "No data"
                });
            }
            if (sensorRegister6[4] == true)
            {
                SensorStatuses.Add(new SensorStatus
                {
                    Code = "DFCSALM",
                    Description = "DFCS changed greatly"
                });
            }
            if (sensorRegister6[5] == true)
            {
                SensorStatuses.Add(new SensorStatus
                {
                    Code = "ALARM",
                    Description = "Sensor level down"
                });
            }
            if (sensorRegister6[6] == true)
            {
                SensorStatuses.Add(new SensorStatus
                {
                    Code = "Spb6",
                    Description = "Special specification (Parameter of initialization command)"
                });
            }
            if (sensorRegister6[7] == true)
            {
                SensorStatuses.Add(new SensorStatus
                {
                    Code = "Spb7",
                    Description = "Special specification (Parameter of initialization command)"
                });
            }
        }
    }
}
