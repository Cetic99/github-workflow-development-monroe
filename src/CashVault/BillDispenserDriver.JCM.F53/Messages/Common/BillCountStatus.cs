namespace CashVault.BillDispenserDriver.JCM.F53.Messages.Common
{
    public class BillCountStatus
    {
        Dictionary<int, byte> IsoCode =
        new Dictionary<int, byte>() {
            {0x30, 0},
            {0xB1, 1},
            {0xB2, 2},
            {0x33, 3},
            {0xB4, 4},
            {0x35, 5},
            {0x36, 6},
            {0xB7, 7},
            {0xB8, 8},
            {0x39, 9}
        };

        public int CassetteNumber { get; init; }

        public int NumOfCountedBills { get; init; }

        public int NumOfBillRejections { get; init; }

        public int NumOfBillsToBeCountedCommand { get; init; }

        public int MaxNumOfCountRejectCommand { get; init; }

        public int MaxNumOfPickRetriesCommand { get; init; }

        CassetteStatisticalInformation? StatisticalInformation;


        public BillCountStatus(int cassetteId, byte[] countedBills, byte[] billRejections, byte[] billsToBeCounted,
                               byte[] countReject, int pickRetries, byte[] statisticalInformation)
        {
            CassetteStatisticalInformation StatisticalInformation = new CassetteStatisticalInformation(statisticalInformation);

            CassetteNumber = cassetteId;
            try
            {
                NumOfCountedBills = IsoCodeConversion(countedBills);
                NumOfBillRejections = IsoCodeConversion(billRejections);
                NumOfBillsToBeCountedCommand = IsoCodeConversion(billsToBeCounted);
                MaxNumOfCountRejectCommand = IsoCodeConversion(countReject);
            }
            catch (Exception ex)
            {
                // Handle any exceptions from the method
            }

            MaxNumOfPickRetriesCommand = pickRetries;

        }

        //Method for ISO code conversion
        public int IsoCodeConversion(byte[] values)
        {
            // Ensure the input array has exactly two bytes
            if (values.Length != 2)
                throw new ArgumentException("Input array must contain exactly two bytes.");

            // Check if both values exist in the dictionary
            if (!IsoCode.ContainsKey(values[0]))
                throw new KeyNotFoundException("Value not found in the dictionary: " + values[0].ToString("X"));

            if (!IsoCode.ContainsKey(values[1]))
                throw new KeyNotFoundException("Value not found in the dictionary: " + values[1].ToString("X"));

            // Compute the result
            int result = 10 * IsoCode[values[0]] + IsoCode[values[1]];

            // Check if result is greater than 20 because it is maximum per cassette in one transaction
            if (result > 20)
                throw new InvalidOperationException("Result is greater than 20.");

            return result;
        }
    }
}
