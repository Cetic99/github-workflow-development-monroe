using System.Windows;
using System.Text.RegularExpressions;

namespace NFC_card_tool;

public partial class WriteGuidWindow : Window
{
    private readonly NfcReaderService _nfcService;
    private readonly string _readerName;
    private readonly CardInfo? _cardInfo;
    private string? _generatedGuid;
    private string? _serialNumber;

    public string? WrittenGuid => _generatedGuid;
    public string? WrittenSerialNumber => _serialNumber;
    public string? WrittenData => !string.IsNullOrEmpty(_serialNumber) && !string.IsNullOrEmpty(_generatedGuid)
        ? $"{_serialNumber}-{_generatedGuid}"
        : null;

    public WriteGuidWindow(NfcReaderService nfcService, string readerName, CardInfo? cardInfo)
    {
        InitializeComponent();

        _nfcService = nfcService;
        _readerName = readerName;
        _cardInfo = cardInfo;

        InitializeCardInfo();
    }

    private void InitializeCardInfo()
    {
        if (_cardInfo != null)
        {
            CardUidText.Text = _cardInfo.UID;
            CardTypeText.Text = _cardInfo.CardType;

            // Update info text based on card type
            bool isNtag = _cardInfo.CardType.Contains("NTAG");
            if (isNtag)
            {
                WriteInfoText.Text =
               "- Serial number and GUID will be written to user memory (Pages 4-15)\n" +
                   "- NTAG cards use 4-byte pages\n" +
                     "- Format: SERIAL-GUID\n" +
                     "- This data will uniquely identify the user\n" +
                "- Save both values to your database for authentication\n" +
                "- Card must remain on reader during write operation";
            }
            else
            {
                WriteInfoText.Text =
                   "- Serial number and GUID will be written to user memory (Blocks 4-10)\n" +
                "- MIFARE Classic uses 16-byte blocks\n" +
                  "- Format: SERIAL-GUID\n" +
               "- This data will uniquely identify the user\n" +
              "- Save both values to your database for authentication\n" +
                    "- Card must remain on reader during write operation";
            }

            if (_cardInfo.IsLocked)
            {
                CardStatusText.Text = "LOCKED - Cannot write";
                CardStatusText.Foreground = System.Windows.Media.Brushes.Red;
                WriteButton.IsEnabled = false;
            }
            else
            {
                CardStatusText.Text = "Ready for writing";
                CardStatusText.Foreground = System.Windows.Media.Brushes.Green;
            }
        }
        else
        {
            CardStatusText.Text = "Unknown status";
            WriteButton.IsEnabled = false;
        }
    }

    private void SerialNumberTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        // Only allow digits
        var textBox = (System.Windows.Controls.TextBox)sender;
        var text = textBox.Text;

        // Remove non-digit characters
        var digitsOnly = Regex.Replace(text, @"[^\d]", "");

        if (digitsOnly != text)
        {
            var selectionStart = textBox.SelectionStart;
            textBox.Text = digitsOnly;
            textBox.SelectionStart = Math.Min(selectionStart, digitsOnly.Length);
        }

        // Update serial number
        _serialNumber = digitsOnly.Length == 8 ? digitsOnly : null;

        // Update hint text
        if (string.IsNullOrEmpty(digitsOnly))
        {
            SerialNumberHintText.Text = "Enter 8-digit serial number (e.g., 12345678)";
            SerialNumberHintText.Foreground = System.Windows.Media.Brushes.Gray;
        }
        else if (digitsOnly.Length < 8)
        {
            SerialNumberHintText.Text = $"Need {8 - digitsOnly.Length} more digit(s)";
            SerialNumberHintText.Foreground = System.Windows.Media.Brushes.Orange;
        }
        else if (digitsOnly.Length == 8)
        {
            SerialNumberHintText.Text = "Serial number valid";
            SerialNumberHintText.Foreground = System.Windows.Media.Brushes.Green;
        }

        // Update write button state
        UpdateWriteButtonState();
    }

    private void GenerateGuidButton_Click(object sender, RoutedEventArgs e)
    {
        // Generate new GUID
        _generatedGuid = Guid.NewGuid().ToString();
        GuidTextBox.Text = _generatedGuid;

        // Update write button state
        UpdateWriteButtonState();
    }

    private void UpdateWriteButtonState()
    {
        // Enable write button only if:
        // 1. Serial number is valid (8 digits)
        // 2. GUID is generated
        // 3. Card is not locked
        WriteButton.IsEnabled = !string.IsNullOrEmpty(_serialNumber) &&
              !string.IsNullOrEmpty(_generatedGuid) &&
        _cardInfo != null &&
              !_cardInfo.IsLocked;
    }

    private async void WriteButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_serialNumber))
        {
            MessageBox.Show("Please enter an 8-digit serial number!", "No Serial Number",
     MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrEmpty(_generatedGuid))
        {
            MessageBox.Show("Please generate a GUID first!", "No GUID",
                  MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var combinedData = $"{_serialNumber}-{_generatedGuid}";

        var result = MessageBox.Show(
          $"Are you sure you want to write this data to the card?\n\n" +
                 $"Serial Number: {_serialNumber}\n" +
               $"GUID: {_generatedGuid}\n\n" +
          $"Combined: {combinedData}\n\n" +
           "This will modify the card's user memory.\n" +
         "Make sure to save both values to your database!",
           "Confirm Write Operation",
         MessageBoxButton.YesNo,
     MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        // Disable buttons during write
        WriteButton.IsEnabled = false;
        GenerateGuidButton.IsEnabled = false;
        SerialNumberTextBox.IsEnabled = false;
        ProgressPanel.Visibility = Visibility.Visible;

        try
        {
            // Write combined data (serial-GUID) to card
            var success = await Task.Run(() => _nfcService.WriteDataToCard(_readerName, combinedData));

            if (success)
            {
                string locationInfo = _cardInfo?.CardType.Contains("NTAG") == true
      ? "pages 4-15"
: "blocks 4-10";

                var copyResult = MessageBox.Show(
   $"Data successfully written to card!\n\n" +
     $"Serial Number: {_serialNumber}\n" +
  $"GUID: {_generatedGuid}\n\n" +
       $"Written to {locationInfo}\n" +
         "Ready for authentication\n\n" +
            "IMPORTANT: Save both values to your database!\n\n" +
    "Do you want to copy this data to clipboard?",
       "Write Successful",
  MessageBoxButton.YesNo,
  MessageBoxImage.Information);

                if (copyResult == MessageBoxResult.Yes)
                {
                    CopyToClipboard();
                }

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show(
               "Failed to write data to card.\n\n" +
                      "Possible reasons:\n" +
                 "• Card was removed during write\n" +
                 "• Card is write-protected\n" +
                 "• Authentication failed\n\n" +
                    "Check the Debug Log for details.",
                    "Write Failed",
                      MessageBoxButton.OK,
                  MessageBoxImage.Error);

                WriteButton.IsEnabled = true;
                GenerateGuidButton.IsEnabled = true;
                SerialNumberTextBox.IsEnabled = true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
       $"Error during write operation:\n\n{ex.Message}",
          "Error",
    MessageBoxButton.OK,
                  MessageBoxImage.Error);

            WriteButton.IsEnabled = true;
            GenerateGuidButton.IsEnabled = true;
            SerialNumberTextBox.IsEnabled = true;
        }
        finally
        {
            ProgressPanel.Visibility = Visibility.Collapsed;
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void CopyToClipboard()
    {
        var info = new System.Text.StringBuilder();

        info.AppendLine("NFC CARD WRITE RESULTS");
        info.AppendLine(new string('=', 50));
        info.AppendLine();

        info.AppendLine("CARD IDENTIFICATION");
        info.AppendLine($"  UID: {_cardInfo?.UID}");
        info.AppendLine($"  Card Type: {_cardInfo?.CardType}");
        info.AppendLine();

        info.AppendLine("WRITTEN DATA");
        info.AppendLine($"  Serial Number: {_serialNumber}");
        info.AppendLine($"  GUID: {_generatedGuid}");
        info.AppendLine($"  Combined: {_serialNumber}-{_generatedGuid}");
        info.AppendLine();

        string locationInfo = _cardInfo?.CardType.Contains("NTAG") == true
     ? "Pages 4-15 (User Memory)"
        : "Blocks 4-10 (User Memory)";
        info.AppendLine($"  Location: {locationInfo}");
        info.AppendLine();

        info.AppendLine("DATABASE INSERT EXAMPLES");
        info.AppendLine(new string('-', 50));
        info.AppendLine();
        info.AppendLine("SQL:");
        info.AppendLine($"  INSERT INTO Users (SerialNumber, GUID, CreatedDate)");
        info.AppendLine($"  VALUES ('{_serialNumber}', '{_generatedGuid}', GETDATE());");
        info.AppendLine();
        info.AppendLine("JSON:");
        info.AppendLine("  {");
        info.AppendLine($"  \"serialNumber\": \"{_serialNumber}\",");
        info.AppendLine($" \"guid\": \"{_generatedGuid}\",");
        info.AppendLine($"    \"combined\": \"{_serialNumber}-{_generatedGuid}\"");
        info.AppendLine("  }");
        info.AppendLine();

        info.AppendLine(new string('=', 50));
        info.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        try
        {
            Clipboard.SetText(info.ToString());
            MessageBox.Show(
       "Data copied to clipboard!\n\n" +
             "You can now paste it into your database or documentation.",
           "Copied Successfully",
         MessageBoxButton.OK,
              MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
            $"Failed to copy to clipboard:\n\n{ex.Message}",
             "Copy Failed",
         MessageBoxButton.OK,
             MessageBoxImage.Error);
        }
    }
}
