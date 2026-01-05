using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.Common
{
    public class BillCountConfiguration
    {
        public BillCountRequestMessageItem[]? BillCount { get; set; }

        public BillCountConfiguration() { }

        public class BillCountRequestMessageItem
        {
            public int CassetteId { get; init; }
            public int Count { get; init; }
            public int MaxNumberOfCountReject { get; init; }
            public int PickRetriesOfCount { get; init; }
        }
    }
}
