using CashVault.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CashVault.Domain.ValueObjects.Currency;

namespace CashVault.TicketPrinterDriver.FutureLogic.Commands;

internal class PrintTemplate0Command : BaseCommand
{
    public string Caption { get; }
    public string TicketBarcode { get; }
    public decimal Amount { get; }
    public Currency Currency { get; }
    public string LocationName { get; }
    public string LocationAddress { get; }
    public int DaysValid { get; }
    public DateTime PrintingTimestamp { get; }
    public string? TicketNumber { get; }
    public string MachineNumber { get; }

    public PrintTemplate0Command(string caption, string ticketBarcode, decimal amount, Currency currency, string locationName, string locationAddress, DateTime printingTimestamp, int daysValid, string? ticketNumber = "", string machineNumber = "") : base()
    {
        if (ticketBarcode.Length != 18)
        {
            throw new ArgumentException("Ticket barcode must be 18 characters long.", nameof(ticketBarcode));
        }
        Caption = caption;
        TicketBarcode = ticketBarcode;
        Amount = amount;
        Currency = currency;
        LocationName = locationName;
        LocationAddress = locationAddress;
        DaysValid = daysValid;
        PrintingTimestamp = printingTimestamp;
        TicketNumber = ticketNumber;
        MachineNumber = machineNumber;
    }

    public override byte[] GetMessageBytes()
    {
        string formattedPrintingTimestamp = PrintingTimestamp.ToString("yyyy-dd-MM|HH:mm:ss");
        string barcodeWithDashes = $"{TicketBarcode.Substring(0, 2)}-{TicketBarcode.Substring(2, 4)}-{TicketBarcode.Substring(6, 4)}-{TicketBarcode.Substring(10, 4)}-{TicketBarcode.Substring(14, 4)}";
        string commandText = "^P|0|1|";
        commandText += $"{barcodeWithDashes}|";
        commandText += $"{Caption}|";
        commandText += $"{LocationName}|";
        commandText += $"{LocationAddress}|||";
        commandText += $"{barcodeWithDashes}|";
        commandText += $"{formattedPrintingTimestamp}|";
        commandText += $"TICKET # {TicketNumber}|";
        commandText += $"{ConvertAmountToWords(Amount)}||";

        if (Currency.SymbolPosition == CurrencySymbolPositionEnum.BeforeValue)
        {
            commandText += $"{Currency.Symbol}{Amount:F2}||";
        }
        else
        {
            commandText += $"{Amount:F2}{Currency.Symbol}||";
        }
            
        commandText += $"{DaysValid} days|";
        commandText += $"MACHINE# {MachineNumber}|";
        commandText += $"{TicketBarcode}|^";
        return System.Text.Encoding.ASCII.GetBytes(commandText);
    }

    

    private string ConvertAmountToWords(decimal amount)
    {
        string ConvertIntegerToWords(int number)
        {
            string[] UnitsMap = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten",
                                                  "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen",
                                                  "eighteen", "nineteen" };

            string[] TensMap = { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

            if (number == 0)
                return "zero";

            if (number < 20)
                return UnitsMap[number];

            if (number < 100)
                return TensMap[number / 10] + (number % 10 > 0 ? "-" + UnitsMap[number % 10] : "");

            if (number < 1000)
                return UnitsMap[number / 100] + " hundred" + (number % 100 > 0 ? " and " + ConvertIntegerToWords(number % 100) : "");

            if (number < 1000000)
                return ConvertIntegerToWords(number / 1000) + " thousand" + (number % 1000 > 0 ? " " + ConvertIntegerToWords(number % 1000) : "");

            if (number < 1000000000)
                return ConvertIntegerToWords(number / 1000000) + " million" + (number % 1000000 > 0 ? " " + ConvertIntegerToWords(number % 1000000) : "");

            return ConvertIntegerToWords(number / 1000000000) + " billion" + (number % 1000000000 > 0 ? " " + ConvertIntegerToWords(number % 1000000000) : "");
        }

        if (amount == 0)
            return "ZERO";

        int integerPart = (int)Math.Floor(amount);
        int decimalPart = (int)((amount - integerPart) * 100);

        string words = ConvertIntegerToWords(integerPart);

        if (decimalPart > 0)
        {
            words += " and " + ConvertIntegerToWords(decimalPart) + "";
        }

        return words.Trim().ToUpperInvariant();
    }

    

    //private string ConvertAmountToWords(decimal amount)
    //{
    //    // Define the number names for each digit
    //    string[] singleDigits = { "", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
    //    string[] teenDigits = { "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
    //    string[] tensDigits = { "", "", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

    //    // Define the number names for each place value
    //    string[] placeValues = { "", "thousand", "million", "billion", "trillion", "quadrillion", "quintillion" };

    //    // Convert the amount to words
    //    string amountInWords = "";

    //    // Handle zero amount
    //    if (amount == 0)
    //    {
    //        amountInWords = "zero";
    //    }
    //    else
    //    {
    //        // Split the amount into groups of three digits
    //        string amountString = amount.ToString("F2").Replace(".", "");
    //        int groupCount = (amountString.Length + 2) / 3;
    //        int[] groups = new int[groupCount];
    //        for (int i = 0; i < groupCount; i++)
    //        {
    //            groups[i] = int.Parse(amountString.Substring(Math.Max(amountString.Length - (i + 1) * 3, 0), Math.Min(amountString.Length - i * 3, 3)));
    //        }

    //        // Convert each group to words
    //        for (int i = 0; i < groupCount; i++)
    //        {
    //            int group = groups[i];
    //            if (group > 0)
    //            {
    //                string groupInWords = "";

    //                // Convert hundreds place
    //                int hundreds = group / 100;
    //                if (hundreds > 0)
    //                {
    //                    groupInWords += singleDigits[hundreds] + " hundred ";
    //                }

    //                // Convert tens and ones places
    //                int tens = group % 100;
    //                if (tens >= 10 && tens <= 19)
    //                {
    //                    groupInWords += teenDigits[tens - 10];
    //                }
    //                else
    //                {
    //                    int ones = tens % 10;
    //                    if (tens >= 20)
    //                    {
    //                        groupInWords += tensDigits[tens / 10] + " ";
    //                    }
    //                    if (ones > 0)
    //                    {
    //                        groupInWords += singleDigits[ones] + " ";
    //                    }
    //                }

    //                // Add place value
    //                groupInWords += placeValues[groupCount - i - 1] + " ";

    //                // Append group to the amount in words
    //                amountInWords += groupInWords;
    //            }
    //        }
    //    }

    //    return amountInWords.Trim().ToUpperInvariant();
    //}

}
