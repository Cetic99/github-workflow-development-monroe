using CashVault.DeviceDriver.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.CabinetControlUnitDriver.Interfaces
{
    internal interface ICabinetControlUnit : ISerialPortMessage
    {
        /// <summary>
        /// Checks if the response is a valid message.
        /// </summary>
        /// <returns>True if the response is a valid message, otherwise false.</returns>
        bool IsValidMessage();
    }
}
