using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.TicketPrinterDriver.FutureLogic.Commands;

internal class PrintTextCommand : BaseCommand
{
    private string CommandText { get; }
    public PrintTextCommand(string[] lines)
    {
        const char ESC = (char)0x1B;
        const char CR = (char)0x0D;
        const char LF = (char)0x0A;
        const char FF = (char)0x0C;
        int font = 7; // selected font (Table 7-1 datasheet)

        // switch to journal mode for text printing
        CommandText = $"^j|^{ESC}[F{font}";

        // add lines to command text
        foreach (var line in lines)
        {
            CommandText += line;
            CommandText += $"{CR}{LF}";
        }

        // complete printing and switch back to TCL mode
        CommandText += $"{FF}{ESC}[^]{ESC}";
    }
    public override byte[] GetMessageBytes()
    {
        return System.Text.Encoding.ASCII.GetBytes(CommandText);
    }
}
