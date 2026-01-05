using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillDispenserDriver.JCM.F53.Interfaces
{
    /// <summary>
    /// Represents a response from the bill dispenser.
    /// </summary>
    internal interface IBillDispenserResponse : IBillDispenserMessage
    {
        /// <summary>
        /// Checks if the response is positive.
        /// </summary>
        /// <returns>True if the response is positive, otherwise false.</returns>
        bool IsPositive();

        /// <summary>
        /// Checks if the response is a valid message.
        /// </summary>
        /// <returns>True if the response is a valid message, otherwise false.</returns>
        bool IsValidMessage();
    }
}
