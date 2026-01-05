using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.CabinetControlUnitDriver.Messages.Common
{
    internal class CabinetControlUnitError
    {
        public byte? Code { get; init; }
        public string? Description { get; init; }
    }
    internal class CabinetControlUnitWarning
    {
        public byte? Code { get; init; }
        public string? Description { get; init; }
    }
}
