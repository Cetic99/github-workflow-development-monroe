using CashVault.BillDispenserDriver.JCM.F53.Messages.Common;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.RequestMessages
{

    public class BillCountRequestMessage : EnhancedRequestMessage
    {

        private static readonly Dictionary<int, byte[]> numBillsToCount = new Dictionary<int, byte[]>
        {
            {0, [0x30, 0x30]}, {1, [0x30, 0xB1]}, {2, [0x30, 0xB2]}, {3, [0x30, 0x33]}, {4, [0x30, 0xB4]},
            {5, [0x30, 0x35]}, {6, [0x30, 0x36]}, {7, [0x30, 0xB7]}, {8, [0x30, 0xB8]}, {9, [0x30, 0x39]},
            {10,[0xB1, 0x30]}, {11,[0xB1, 0xB1]}, {12,[0xB1, 0xB2]}, {13,[0xB1, 0x33]}, {14,[0xB1, 0xB4]},
            {15,[0xB1, 0x35]}, {16,[0xB1, 0x36]}, {17,[0xB1, 0xB7]}, {18,[0xB1, 0xB8]}, {19,[0xB1, 0x39]},
            {20,[0xB2, 0x30]},

        };

        public byte DH1;
        public byte RSV;
        public byte[] DH3;
        public byte[] ODR;

        public class BillCountSpecification
        {
            public int CassetteId { get; init; }
            public byte N1 { get; init; }
            public byte N2 { get; init; }
            public byte R1 { get; init; }
            public byte R2 { get; init; }
            public byte P { get; init; }
            public BillCountSpecification(int cassetteId, int count, int maxNumberOfCountReject, int pickRetriesOfCount)
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThan(count, 20);
                ArgumentOutOfRangeException.ThrowIfGreaterThan(maxNumberOfCountReject, 20);
                ArgumentOutOfRangeException.ThrowIfGreaterThan(pickRetriesOfCount, 15);

                this.CassetteId = cassetteId;
                byte[] N = numBillsToCount[count];
                this.N1 = N[0];
                this.N2 = N[1];

                byte[] R = numBillsToCount[maxNumberOfCountReject];
                this.R1 = R[0];
                this.R2 = R[1];

                this.P = (byte)pickRetriesOfCount;
            }
        }

        private BillCountSpecification[] billCountSpec = new BillCountSpecification[8];

        public BillCountRequestMessage(BillCountConfiguration configuration)
        {
            DH1 = 0x03; //DH1 is 0x09 in the datasheet but it is not working!
            RSV = 0x00;
            DH3 = [0x00, 0x2C];
            ODR = [0xFE, 0xDC, 0xBA, 0x98];

            for (int i = 0; i < 8; i++)
            {
                var configSpec = configuration.BillCount.FirstOrDefault(x => x.CassetteId == i + 1);
                if (configSpec == null)
                {
                    // Cassette not configured
                    billCountSpec[i] = new BillCountSpecification(i, 0, 0, 0);
                }
                else
                {
                    billCountSpec[i] = new BillCountSpecification(i, configSpec.Count, configSpec.MaxNumberOfCountReject, configSpec.PickRetriesOfCount);
                }
            }
        }

        public override byte[] GetMessageBytes()
        {
            byte[] message =
            [
                DH0,
                DH1,
                DH2,
                RSV,
                DH3[0],
                DH3[1],
                ODR[0],
                ODR[1],
                ODR[2],
                ODR[3]
            ];

            byte[] billCountFirstPartN = billCountSpec.Where(item => item.CassetteId < 4).Select(item => new byte[] { item.N1, item.N2 }).SelectMany(item => item).ToArray();
            byte[] billCountFirstPartR = billCountSpec.Where(item => item.CassetteId < 4).Select(item => new byte[] { item.R1, item.R2 }).SelectMany(item => item).ToArray();
            byte[] billCountFirstPartP = billCountSpec.Where(item => item.CassetteId < 4).Select(item => new byte[] { item.P }).SelectMany(item => item).ToArray();

            byte[] billCountSecondPartN = billCountSpec.Where(item => item.CassetteId > 3).Select(item => new byte[] { item.N1, item.N2 }).SelectMany(item => item).ToArray();
            byte[] billCountSecondPartR = billCountSpec.Where(item => item.CassetteId > 3).Select(item => new byte[] { item.R1, item.R2 }).SelectMany(item => item).ToArray();
            byte[] billCountSecondPartP = billCountSpec.Where(item => item.CassetteId > 3).Select(item => new byte[] { item.P }).SelectMany(item => item).ToArray();

            message = ConcatenateByteArrays(message, billCountFirstPartN);
            message = ConcatenateByteArrays(message, billCountFirstPartR);
            message = ConcatenateByteArrays(message, billCountFirstPartP);
            message = ConcatenateByteArrays(message, billCountSecondPartN);
            message = ConcatenateByteArrays(message, billCountSecondPartR);
            message = ConcatenateByteArrays(message, billCountSecondPartP);
            message = ConcatenateByteArrays(message, new byte[] { FS });

            return message;
        }

        private byte[] ConcatenateByteArrays(byte[] array1, byte[] array2)
        {
            byte[] result = new byte[array1.Length + array2.Length];
            Buffer.BlockCopy(array1, 0, result, 0, array1.Length);
            Buffer.BlockCopy(array2, 0, result, array1.Length, array2.Length);
            return result;
        }
    }
}