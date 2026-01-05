

using CashVault.BillDispenserDriver.JCM.F53.Config;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.RequestMessages;

public class BillDispenserDeviceInitializationRequestMessage : EnhancedRequestMessage
{
    public class CasseteByteSpecification
    {
        public int casseteId { get; init; }
        public byte L1 { get; init; }
        public byte L2 { get; init; }
        public byte T { get; init; }

        public CasseteByteSpecification(int casseteId, Int16 length, byte thickness)
        {
            this.casseteId = casseteId;

            Int16 billLengthLong = (short)(length + 5);
            Int16 billLengthShort = (short)(length - 5);

            this.L1 = (byte)((length != 0 & thickness != 0) ? (byte)billLengthLong : 0);
            this.L2 = (byte)((length != 0 & thickness != 0) ? (byte)billLengthShort : 0);
            this.T = thickness;
        }
    }

    public byte DH1;
    public byte RSV1;
    public byte[] DH3;
    public byte[] ODR;

    private CasseteByteSpecification[] casseteSpecification = new CasseteByteSpecification[8];

    private void ValidateDispenserConfiguration(BillDispenserJcm53Configuration configuration)
    {
        // Maximum of 8 cassettes are supported
        ArgumentNullException.ThrowIfNull(configuration.BillCassettes);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(configuration.BillCassettes.Count, 8);

        foreach (var i in (int[])[1, 2, 3, 4, 5, 6])
        {
            if (i >= configuration.BillCassettes.Count) break;

            if (i == 2)
            {
                // Bill dimensions need to be in specific ranges
                ArgumentOutOfRangeException.ThrowIfGreaterThan(configuration.BillCassettes[i].BillDenomination.Length, 180);
                ArgumentOutOfRangeException.ThrowIfLessThan(configuration.BillCassettes[i].BillDenomination.Length, 78);

                ArgumentOutOfRangeException.ThrowIfGreaterThan(configuration.BillCassettes[i].BillDenomination.Thickness, 30);
                ArgumentOutOfRangeException.ThrowIfLessThan(configuration.BillCassettes[i].BillDenomination.Thickness, 9);
            }

            // Bill dimensions need to be in specific ranges
            ArgumentOutOfRangeException.ThrowIfGreaterThan(configuration.BillCassettes[i].BillDenomination.Length, 187);
            ArgumentOutOfRangeException.ThrowIfLessThan(configuration.BillCassettes[i].BillDenomination.Length, 110);

            ArgumentOutOfRangeException.ThrowIfGreaterThan(configuration.BillCassettes[i].BillDenomination.Thickness, 20);
            ArgumentOutOfRangeException.ThrowIfLessThan(configuration.BillCassettes[i].BillDenomination.Thickness, 9);

        }

    }

    public BillDispenserDeviceInitializationRequestMessage(BillDispenserJcm53Configuration configuration)
    {
        ValidateDispenserConfiguration(configuration);

        DH1 = 0x02;
        RSV1 = 0x00;
        DH3 = [0x00, 0x1A];
        ODR = [0x00, 0x00];

        for (int i = 0; i < 8; i++)
        {
            var configSpec = configuration.BillCassettes.FirstOrDefault(x => x.CassetteNumber == i + 1);
            if (configSpec == null)
            {
                // Cassette not configured
                casseteSpecification[i] = new CasseteByteSpecification(i, 0, 0);
            }
            else
            {
                casseteSpecification[i] = new CasseteByteSpecification(i, (short)configSpec.BillDenomination.Length, (byte)configSpec.BillDenomination.Thickness);
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
            RSV1,
            DH3[0],
            DH3[1],
            ODR[0],
            ODR[1]
        ];

        byte[] cassetteBytesFirstPartLength = casseteSpecification.Where(item => item.casseteId < 4).Select(item => new byte[] { item.L1, item.L2 }).SelectMany(item => item).ToArray();
        byte[] cassetteBytesFirstPartThickness = casseteSpecification.Where(item => item.casseteId < 4).Select(item => new byte[] { item.T }).SelectMany(item => item).ToArray();

        byte[] cassetteBytesSecondPartLength = casseteSpecification.Where(item => item.casseteId > 3).Select(item => new byte[] { item.L1, item.L2 }).SelectMany(item => item).ToArray();
        byte[] cassetteBytesSecondPartThickness = casseteSpecification.Where(item => item.casseteId > 3).Select(item => new byte[] { item.T }).SelectMany(item => item).ToArray();

        message = ConcatenateByteArrays(message, cassetteBytesFirstPartLength);
        message = ConcatenateByteArrays(message, cassetteBytesFirstPartThickness);
        message = ConcatenateByteArrays(message, cassetteBytesSecondPartLength);
        message = ConcatenateByteArrays(message, cassetteBytesSecondPartThickness);
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

