using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.Common
{
    internal class BillCassetteMagnetStatus
    {
        public bool MagnetA { get; init; }
        public bool MagnetB { get; init; }
        public bool MagnetC { get; init; }
        public bool MagnetD { get; init; }

        public BillCassetteMagnetStatus(bool magnetA = false, bool magnetB = false, bool magnetC = false, bool magnetD = false)
        {
            MagnetA = magnetA;
            MagnetB = magnetB;
            MagnetC = magnetC;
            MagnetD = magnetD;
        }
    }
}
