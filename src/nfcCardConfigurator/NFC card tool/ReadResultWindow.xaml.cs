using System.Windows;

namespace NFC_card_tool;

public partial class ReadResultWindow : Window
{
    private readonly CardInfo _cardInfo;
    private readonly string? _userData;

    public ReadResultWindow(CardInfo cardInfo, string? userData)
    {
        InitializeComponent();

        _cardInfo = cardInfo;
        _userData = userData;

        PopulateData();
    }

    private void PopulateData()
    {
        // Identification
        UidText.Text = _cardInfo.UID;
        UidLengthText.Text = $"{_cardInfo.UIDLength} bytes ({GetUIDType(_cardInfo.UIDLength)})";
        CardTypeText.Text = _cardInfo.CardType;

        // Technical Specifications
        AtrText.Text = _cardInfo.ATR;
        ProtocolText.Text = _cardInfo.Protocol;
        MemoryText.Text = !string.IsNullOrEmpty(_cardInfo.MemorySize)
    ? _cardInfo.MemorySize
 : "Unknown";

        // Card Status
        FactoryStateText.Text = _cardInfo.IsFactoryState
          ? "YES - Card is blank/unused"
     : "NO - Card contains user data";
        // Remove color coding - use default text color
        // FactoryStateText.Foreground remains default (same as other text)

        LockStatusText.Text = _cardInfo.IsLocked
     ? "LOCKED - Write protected"
     : "UNLOCKED - Writable";
        LockStatusText.Foreground = _cardInfo.IsLocked
      ? System.Windows.Media.Brushes.Red
     : System.Windows.Media.Brushes.Green;

        if (_cardInfo.BlockData != null)
        {
            UsedBlocksText.Text = $"{_cardInfo.UsedBlocks} of {_cardInfo.BlockData.Count} blocks";
        }
        else
        {
            UsedBlocksText.Text = "N/A";
        }

        // User Data
        if (!string.IsNullOrEmpty(_userData))
        {
            NoDataPanel.Visibility = Visibility.Collapsed;
            UserDataPanel.Visibility = Visibility.Visible;

            // Try to decode user data as ASCII
            string? decodedText = TryDecodeUserData(_userData);

            if (!string.IsNullOrEmpty(decodedText))
            {
                // Check if it's in SERIAL-GUID format
                var parts = decodedText.Split('-');
                if (parts.Length >= 6 && parts[0].Length == 8 && parts[0].All(char.IsDigit))
                {
                    // Format: SERIAL-GUID
                    var serialNumber = parts[0];
                    var guid = string.Join("-", parts.Skip(1));

                    var formatted = new System.Text.StringBuilder();
                    formatted.AppendLine("DECODED USER DATA");
                    formatted.AppendLine(new string('=', 50));
                    formatted.AppendLine();
                    formatted.AppendLine($"Serial Number: {serialNumber}");
                    formatted.AppendLine($"GUID: {guid}");
                    formatted.AppendLine();
                    formatted.AppendLine(new string('=', 50));
                    formatted.AppendLine("RAW HEX DATA");
                    formatted.AppendLine(new string('=', 50));
                    formatted.AppendLine();
                    formatted.Append(FormatUserData(_userData));

                    UserDataTextBox.Text = formatted.ToString();
                }
                else
                {
                    // Regular decoded text
                    var formatted = new System.Text.StringBuilder();
                    formatted.AppendLine("DECODED USER DATA");
                    formatted.AppendLine(new string('=', 50));
                    formatted.AppendLine();
                    formatted.AppendLine(decodedText);
                    formatted.AppendLine();
                    formatted.AppendLine(new string('=', 50));
                    formatted.AppendLine("RAW HEX DATA");
                    formatted.AppendLine(new string('=', 50));
                    formatted.AppendLine();
                    formatted.Append(FormatUserData(_userData));

                    UserDataTextBox.Text = formatted.ToString();
                }
            }
            else
            {
                // Display user data in hex format only
                UserDataTextBox.Text = FormatUserData(_userData);
            }

            string location = _cardInfo.CardType.Contains("NTAG")
           ? "Pages 4-15 (User Memory)"
          : "Blocks 4-9 (User Memory)";
            DataLocationText.Text = location;

            UserDataBorder.Background = new System.Windows.Media.SolidColorBrush(
           System.Windows.Media.Color.FromRgb(0xD1, 0xF2, 0xED)); // Light green (same as Write window)
        }
        else
        {
            NoDataPanel.Visibility = Visibility.Visible;
            UserDataPanel.Visibility = Visibility.Collapsed;
        }
    }

    private string? TryDecodeUserData(string hexData)
    {
        try
        {
            var bytes = hexData.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            var byteArray = new List<byte>();

            foreach (var b in bytes)
            {
                if (byte.TryParse(b, System.Globalization.NumberStyles.HexNumber, null, out byte value))
                {
                    byteArray.Add(value);
                }
            }

            // Find actual data end (before padding nulls/FF)
            int dataEnd = byteArray.Count;
            for (int i = 0; i < byteArray.Count; i++)
            {
                if (byteArray[i] == 0x00 || byteArray[i] == 0xFF)
                {
                    // Check if rest is padding
                    bool restIsPadding = true;
                    for (int j = i; j < byteArray.Count; j++)
                    {
                        if (byteArray[j] != 0x00 && byteArray[j] != 0xFF)
                        {
                            restIsPadding = false;
                            break;
                        }
                    }
                    if (restIsPadding)
                    {
                        dataEnd = i;
                        break;
                    }
                }
            }

            var actualData = byteArray.Take(dataEnd).ToArray();
            var decoded = System.Text.Encoding.ASCII.GetString(actualData);

            // Check if it's printable ASCII
            if (decoded.All(c => char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsWhiteSpace(c) || c == '-'))
            {
                return decoded;
            }
        }
        catch { }

        return null;
    }

    private string FormatUserData(string hexData)
    {
        var formatted = new System.Text.StringBuilder();

        // Split by spaces and format in rows of 16 bytes
        var bytes = hexData.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < bytes.Length; i += 16)
        {
            // Offset
            formatted.Append($"{i:X4}: ");

            // Hex bytes
            for (int j = 0; j < 16 && i + j < bytes.Length; j++)
            {
                formatted.Append(bytes[i + j]);
                formatted.Append(" ");

                if (j == 7)
                {
                    formatted.Append(" "); // Extra space in middle
                }
            }

            // Padding if less than 16 bytes
            if (i + 16 > bytes.Length)
            {
                int remaining = 16 - (bytes.Length - i);
                formatted.Append(new string(' ', remaining * 3));
                if (bytes.Length - i <= 8)
                {
                    formatted.Append(" ");
                }
            }

            // ASCII representation
            formatted.Append(" | ");
            for (int j = 0; j < 16 && i + j < bytes.Length; j++)
            {
                if (byte.TryParse(bytes[i + j], System.Globalization.NumberStyles.HexNumber,
             null, out byte b))
                {
                    char c = (b >= 32 && b <= 126) ? (char)b : '.';
                    formatted.Append(c);
                }
                else
                {
                    formatted.Append('.');
                }
            }

            formatted.AppendLine();
        }

        return formatted.ToString();
    }

    private string GetUIDType(int length)
    {
        return length switch
        {
            4 => "Single Size",
            7 => "Double Size",
            10 => "Triple Size",
            _ => "Unknown Size"
        };
    }

    private void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        var info = new System.Text.StringBuilder();

        info.AppendLine("NFC CARD ANALYSIS RESULTS");
        info.AppendLine(new string('=', 50));
        info.AppendLine();

        info.AppendLine("IDENTIFICATION");
        info.AppendLine($"  UID: {_cardInfo.UID}");
        info.AppendLine($"  UID Length: {_cardInfo.UIDLength} bytes");
        info.AppendLine($"  Card Type: {_cardInfo.CardType}");
        info.AppendLine();

        info.AppendLine("TECHNICAL SPECIFICATIONS");
        info.AppendLine($"  ATR: {_cardInfo.ATR}");
        info.AppendLine($"  Protocol: {_cardInfo.Protocol}");
        if (!string.IsNullOrEmpty(_cardInfo.MemorySize))
        {
            info.AppendLine($"  Memory: {_cardInfo.MemorySize}");
        }

        info.AppendLine();

        info.AppendLine("CARD STATUS");
        info.AppendLine($"  Factory State: {(_cardInfo.IsFactoryState ? "YES - Blank" : "NO - Has data")}");
        info.AppendLine($"  Lock Status: {(_cardInfo.IsLocked ? "LOCKED" : "UNLOCKED")}");
        info.AppendLine($"  Used Blocks: {_cardInfo.UsedBlocks}");
        info.AppendLine();

        info.AppendLine("USER DATA");
        if (!string.IsNullOrEmpty(_userData))
        {
            info.AppendLine("Card contains user data:");
            info.AppendLine();
            info.AppendLine(FormatUserData(_userData));
            string location = _cardInfo.CardType.Contains("NTAG")
        ? "Pages 4-12"
                  : "Blocks 4-7";
            info.AppendLine($"Location: {location}");
        }
        else
        {
            info.AppendLine("  No user data found on card");
        }

        Clipboard.SetText(info.ToString());

        MessageBox.Show("Card information copied to clipboard!", "Success",
      MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
