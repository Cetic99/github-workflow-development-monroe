using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.Common
{
    public class CassetteStatisticalInformation
    {
        public int LengthLongErrors { get; set; }
        public int LengthShortErrors { get; init; }
        public int ThicknessErrors { get; init; }
        public int SpacingErrors { get; init; }
        public int PickFromWrongCassetteErrors { get; init; }
        public int PickErrorsWhileBillsNotLow { get; init; }
        public int CountUnmatchErrors { get; init; }
        public int NumberOfPickRetries { get; init; }
        public int NumberOfCountRetriesByCountError { get; init; }
        public int NumberOfRetriesAfterAutoReject { get; init; }
        public int NumberOfJamErrors { get; init; }

        public CassetteStatisticalInformation(byte[] statisticalInfo)
        {

            if (statisticalInfo.Length != 16)
            {
                throw new ArgumentException("Statistical information invalid data length!");
            }
            //TODO: Check values during testing!

            LengthLongErrors = Convert.ToInt32(statisticalInfo[0]);
            LengthShortErrors = Convert.ToInt32(statisticalInfo[1]);
            ThicknessErrors = Convert.ToInt32(statisticalInfo[3]);
            SpacingErrors = Convert.ToInt32(statisticalInfo[4]);
            PickFromWrongCassetteErrors = Convert.ToInt32(statisticalInfo[5]);
            PickErrorsWhileBillsNotLow = Convert.ToInt32(statisticalInfo[6]);
            CountUnmatchErrors = Convert.ToInt32(statisticalInfo[7]);
            NumberOfPickRetries = Convert.ToInt32(statisticalInfo[8]);
            NumberOfCountRetriesByCountError = Convert.ToInt32(statisticalInfo[9]);
            NumberOfRetriesAfterAutoReject = Convert.ToInt32(statisticalInfo[10]);
            NumberOfJamErrors = Convert.ToInt32(statisticalInfo[11]);
        }
    }
}
