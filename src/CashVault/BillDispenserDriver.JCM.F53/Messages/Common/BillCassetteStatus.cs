using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.Common
{
    internal class BillCassetteStatus
    {
        public int CassetteNumber { get; init; }
        public int BillThicknessMillimeters { get; init; }
        public int BillLengthMilimeters { get; init; }
        public int NumberOfStatusChanges { get; init; }
        public List<DispenserError>? CassetteError { get; init; } = new();
        public BillCassetteMagnetStatus CassetteDenomination { get; set; } = new();

        public BillCassetteStatus(int cassetteNumber, int billLength, byte billThickness, byte numberOfStatusChanges, byte statusRegister)
        {
            if (cassetteNumber < 1 || cassetteNumber > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(cassetteNumber), "The cassette number must be between 1 and 8.");
            }

            CassetteNumber = cassetteNumber;

            BillLengthMilimeters = billLength;

            BillThicknessMillimeters = Convert.ToInt32(billThickness);

            NumberOfStatusChanges = Convert.ToInt32(numberOfStatusChanges);

            ProcessCassetteStatusRegister(statusRegister);

            ProcessDenominationBits(statusRegister);

        }

        void ProcessCassetteStatusRegister(byte statusRegister)
        {
            if (CassetteNumber < 1)
            {
                throw new InvalidOperationException("The cassette number must be set before processing the status register.");
            }

            BitArray statusBits = new BitArray(new byte[] { statusRegister });

            if (statusBits[7])
            {
                CassetteError.Add(new DispenserError
                {
                    Code = $"C{CassetteNumber}BLL",
                    Description = $"Cassette {CassetteNumber} bills low.",
                    MaintenanceNeeded = true
                });
            }

            if (statusBits[5])
            {
                CassetteError.Add(new DispenserError
                {
                    Code = $"C{CassetteNumber}PDZ",
                    Description = $"Cassette {CassetteNumber} is not set/denomination is all zero.",
                    MaintenanceNeeded = true
                });
            }

            if (statusBits[4])
            {
                CassetteError.Add(new DispenserError
                {
                    Code = $"C{CassetteNumber}OUT",
                    Description = $"Cassette {CassetteNumber} is not set in place.",
                    IsCritical = true
                });
            }
        }

        void ProcessDenominationBits(byte statusRegister)
        {
            BitArray statusBits = new BitArray(new byte[] { statusRegister });

            CassetteDenomination = new BillCassetteMagnetStatus
            {
                MagnetA = statusBits[3],
                MagnetB = statusBits[2],
                MagnetC = statusBits[1],
                MagnetD = statusBits[0]
            };

            if ((statusBits[3] || statusBits[2] || statusBits[1] || statusBits[0]) == false)
            {
                CassetteError.Add(new DispenserError
                {
                    Code = $"C{CassetteNumber}DN",
                    Description = $"Cassette {CassetteNumber} is drawn out.",
                    IsCritical = true
                });
            }
        }
    }
}
