using CashVault.DeviceDriver.Common;
using CashVault.TicketPrinterDriver.FutureLogic.Common;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace CashVault.TicketPrinterDriver.FutureLogic.Responses;

public class StatusResponse : RAWResponse
{
    public string soft_ver { get; init; }
    private byte status_flags1 { get; init; }
    private byte status_flags2 { get; init; }
    private byte status_flags3 { get; init; }
    private byte status_flags4 { get; init; }
    private byte status_flags5 { get; init; }
    private string temp_numb { get; init; }
    public List<PrinterError> Errors { get; init; } = new();
    public List<PrinterStatusInfo> InformationStatuses { get; init; } = new();
    public const string MsgHeader = "*S|";
    public const string MsgTail = "|*";
    public const int STATUS_MSG_LEN = 29;
    public const int MSG_HEADER_LEN = 3;


    public StatusResponse(byte[] data) : base(data)
    {
        if (data == null || data.Length == 0)
        {
            throw new ArgumentNullException("No response");
        }
        this._data = data;

        if (!(Encoding.ASCII.GetString(data).StartsWith(MsgHeader)))
        {
            throw new ArgumentException("Invalid header");
        }

        if (!(Encoding.ASCII.GetString(data).EndsWith(MsgTail)))
        {
            throw new ArgumentException($"Invalid message tail");
        }

        if (data[3] != '0')
        {
            throw new ArgumentException("Invalid Unit Address");
        }

        soft_ver = Encoding.ASCII.GetString(data.Skip(5).Take(9).ToArray());

        status_flags1 = data[15];
        status_flags2 = data[17];
        status_flags3 = data[19];
        status_flags4 = data[21];
        status_flags5 = data[23];
        temp_numb = Encoding.ASCII.GetString(data.Skip(25).Take(2).ToArray());

        this.parseStatusFlags();
    }

    protected void parseStatusFlags()
    {
        const byte NO_ERR_FLAG = 0x40;
        if ((status_flags1 == NO_ERR_FLAG) &&
            (status_flags2 == NO_ERR_FLAG) &&
            (status_flags3 == NO_ERR_FLAG))
        {
            // Status OK
        }

        BitArray flag1Bitts = new BitArray(new byte[] { status_flags1 });
        BitArray flag2Bitts = new BitArray(new byte[] { status_flags2 });
        BitArray flag3Bitts = new BitArray(new byte[] { status_flags3 });
        BitArray flag4Bitts = new BitArray(new byte[] { status_flags4 });
        BitArray flag5Bitts = new BitArray(new byte[] { status_flags5 });

        // Status flags 1
        if (flag1Bitts[5] == true)
        {
            // system busy
            Errors.Add(new PrinterError
            {
                Code = "SF1_BUSY",
                Description = "Printer busy.",
            });
        }

        if (flag1Bitts[4] == true)
        {
            // system error. General Flag for any system error.
            //Errors.Add(new PrinterError
            //{
            //    Code = "SF1_ERR",
            //    Description = "Printer system error detected.",
            //});
        }

        if (flag1Bitts[3] == true)
        {
            // print head is up
            Errors.Add(new PrinterError
            {
                Code = "SF1_PLATTEN",
                Description = "Print head is up.",
                IsCritical = true,
                MaintenanceNeeded = true,
            });
        }

        if (flag1Bitts[2] == true)
        {
            // paper is out
            Errors.Add(new PrinterError
            {
                Code = "SF1_PAPER",
                Description = "Paper is out.",
                IsCritical = true,
                MaintenanceNeeded = true,
            });
        }

        if (flag1Bitts[1] == true)
        {
            // hardware error. Maintaince needed
            Errors.Add(new PrinterError
            {
                Code = "SF1_HEAD",
                Description = "Printer Hardware error. The printer must be serviced.",
                IsCritical = true,
                MaintenanceNeeded = true,
            });
        }

        if (flag1Bitts[0] == true)
        {
            // voltage error
            Errors.Add(new PrinterError
            {
                Code = "SF1_VOLT",
                Description = "Volatage error detected.",
                IsCritical = false,
                MaintenanceNeeded = false,
            });
        }

        // Status Falgs 2
        if (flag2Bitts[5] == true)
        {
            Errors.Add(new PrinterError
            {
                Code = "SF2_TEMP",
                Description = "High printer head temperature.",
                IsCritical = true,
            });
        }

        if (flag2Bitts[4] == true)
        {
            Errors.Add(new PrinterError
            {
                Code = "SF2_BIT4",
                Description = "Library ref error.",
            });
        }

        if (flag2Bitts[3] == true)
        {
            Errors.Add(new PrinterError
            {
                Code = "SF2_BIT3",
                Description = "PR data error.",
            });
        }

        if (flag2Bitts[2] == true)
        {
            Errors.Add(new PrinterError
            {
                Code = "SF2_BIT2",
                Description = "Library load error.",
            });
        }

        if (flag2Bitts[1] == true)
        {
            Errors.Add(new PrinterError
            {
                Code = "SF2_BUFF",
                Description = "Buffer overflow error.",
                IsCritical = true,
            });
        }

        if (flag2Bitts[0] == true)
        {
            Errors.Add(new PrinterError
            {
                Code = "SF2_JOBMEM",
                Description = "Print job memory overflow.",
                IsCritical = true,
            });
        }

        // Status Falgs 3
        if (flag3Bitts[5] == true)
        {
            Errors.Add(new PrinterError
            {
                Code = "SF3_CMD",
                Description = "Command unsupported or unrecognized. Check command.",
            });
        }

        if (flag3Bitts[4] == true)
        {
            Errors.Add(new PrinterError
            {
                Code = "SF3_LIBSAVE",
                Description = "Print library (fonts) corrupted.",
                IsCritical = true,
            });
        }

        if (flag3Bitts[3] == true)
        {
            InformationStatuses.Add(new PrinterStatusInfo
            {
                Code = "SF3_P_PRESENT",
                Description = "Ticket in paper chute.",
            });
        }

        if (flag3Bitts[2] == true)
        {
            Errors.Add(new PrinterError
            {
                Code = "SF3_BIT2",
                Description = "Flash prog error.",
                IsCritical = true,
            });
        }

        if (flag3Bitts[1] == true)
        {
            Errors.Add(new PrinterError
            {
                Code = "SF3_OL_STAT",
                Description = "Printer offline.",
                IsCritical = true,
            });
        }

        if (flag3Bitts[0] == true)
        {
            Errors.Add(new PrinterError
            {
                Code = "SF3_PS",
                Description = "Wrong paper or error with the Paper Out sensor. Printer is unable to detect the loaded paper’s index mark.",
                IsCritical = true,
            });
        }

        // Status Falgs 4
        if (flag4Bitts[5] == true)
        {
            // reserved
        }

        if (flag4Bitts[4] == true)
        {
            // reserved
        }

        if (flag4Bitts[3] == true)
        {
            // Journal printer mode selected
            InformationStatuses.Add(new PrinterStatusInfo
            {
                Code = "SF4_JNL_MODE",
                Description = "Printer is in Journal mode.",
            });
        }
        else
        {
            // TCL printer mode selected
            InformationStatuses.Add(new PrinterStatusInfo
            {
                Code = "SF4_JNL_MODE",
                Description = "Printer is in TCL mode.",
            });
        }

        if (flag4Bitts[2] == true)
        {
            Errors.Add(new PrinterError
            {
                Code = "SF4_CUTTER",
                Description = "Cutter error or not available.",
            });
        }

        if (flag4Bitts[1] == true)
        {
            /* This flag = 1 if a potential paper jam has occurred.The
            flag will not prevent new tickets from printing unless
            the system has specifically been instructed to do so
            through the ^ z | S command. */
            Errors.Add(new PrinterError
            {
                Code = "SF4_P_JAM",
                Description = "Potential paper jam detected.",
            });
        }

        if (flag4Bitts[0] == true)
        {
            Errors.Add(new PrinterError
            {
                Code = "SF4_P_LOW",
                Description = "Paper low.",
            });
        }

        // Status Falgs 5
        if (flag5Bitts[5] == true)
        {
            // TODO should be checked and analyzed in details. It can provide more information (e.g. power loss)
            //Errors.Add(new PrinterError
            //{
            //    Code = "SF5_BCODE",
            //    Description = "The barcode is not printed completely.",
            //});
        }

        if (flag5Bitts[4] == true)
        {
            // TODO should be checked and analyzed in details. It can provide more information (e.g. power loss)
            //Errors.Add(new PrinterError
            //{
            //    Code = "SF5_TOF",
            //    Description = "The ticket is not printed completely.",
            //});
        }

        if (flag5Bitts[3] == true)
        {
            Errors.Add(new PrinterError
            {
                Code = "SF5_XOFF",
                Description = "The input buffer has exceeded 80% occupancy or is full.",
            });
        }

        if (flag5Bitts[2] == true)
        {
            Errors.Add(new PrinterError
            {
                Code = "SF5_DOOR",
                Description = "The printer drawer is open.",
            });
        }

        if (flag5Bitts[1] == false)
        {
            // TODO should be checked and analyzed in details. It can provide more information (e.g. power loss)
            //Errors.Add(new PrinterError
            //{
            //    Code = "SF5_BCODE_DONE",
            //    Description = "Barcode data processing flag.",
            //});
        }

        if (flag5Bitts[0] == true)
        {
            InformationStatuses.Add(new PrinterStatusInfo
            {
                Code = "SF5_PWR_RST",
                Description = "Printer has been reset or powered up.",
            });
        }
    }
}
