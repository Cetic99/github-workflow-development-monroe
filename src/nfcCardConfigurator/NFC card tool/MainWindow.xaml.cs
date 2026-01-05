using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using System.Text;

namespace NFC_card_tool;

public partial class MainWindow : Window
{
    private readonly NfcReaderService _nfcService;
    private string? _selectedReader;
    private bool _isCardPresent;
    private System.Windows.Threading.DispatcherTimer? _cardCheckTimer;
    private DebugTraceListener? _traceListener;
    private readonly StringBuilder _logBuilder = new();
    private int _messageCount = 0;
    private bool _isDebugPanelOpen = false;

    public MainWindow()
    {
        InitializeComponent();
     _nfcService = new NfcReaderService();

  // Subscribe to events
 _nfcService.CardInserted += OnCardInserted;
 _nfcService.CardRemoved += OnCardRemoved;

      Loaded += MainWindow_Loaded;
        Closing += MainWindow_Closing;

        // Timer for periodic card check
        _cardCheckTimer = new System.Windows.Threading.DispatcherTimer
        {
     Interval = TimeSpan.FromSeconds(1)
        };
   _cardCheckTimer.Tick += CardCheckTimer_Tick;
  
        // Setup debug trace listener
 _traceListener = new DebugTraceListener(this);
        Trace.Listeners.Add(_traceListener);
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
     {
    Debug.WriteLine("=== MainWindow Loading ===");
     await Task.Run(() => _nfcService.Initialize());

   // Link ObservableCollection with ComboBox
     ReaderComboBox.ItemsSource = _nfcService.AvailableReaders;

  if (_nfcService.AvailableReaders.Count > 0)
  {
    ReaderComboBox.SelectedIndex = 0;
 StatusText.Text = "Reader found and ready";
      Debug.WriteLine($"UI: Reader found and ready - {_nfcService.AvailableReaders[0]}");
        }
     else
            {
         StatusText.Text = "No readers available. Connect NFC reader.";
   UpdateConnectionStatus(false);
    Debug.WriteLine("UI: No readers available");
 }
   }
    catch (Exception ex)
   {
    Debug.WriteLine($"UI ERROR: Initialization failed - {ex.Message}");
    MessageBox.Show($"Initialization error:\n\n{ex.Message}",
  "Error", MessageBoxButton.OK, MessageBoxImage.Error);
     StatusText.Text = "Initialization error";
        UpdateConnectionStatus(false);
   }
 }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        _cardCheckTimer?.Stop();
        _nfcService.Dispose();
        
  if (_traceListener != null)
     {
 Trace.Listeners.Remove(_traceListener);
         _traceListener = null;
  }
    }

    private void DebugButton_Click(object sender, RoutedEventArgs e)
    {
  _isDebugPanelOpen = !_isDebugPanelOpen;

        if (_isDebugPanelOpen)
        {
     DebugPanel.Visibility = Visibility.Visible;
  AddDebugLog("Debug panel opened", "INFO");
        }
        else
     {
    DebugPanel.Visibility = Visibility.Collapsed;
        }
    }

    private void CloseDebugButton_Click(object sender, RoutedEventArgs e)
 {
      _isDebugPanelOpen = false;
 DebugPanel.Visibility = Visibility.Collapsed;
    }

    private void ClearDebugButton_Click(object sender, RoutedEventArgs e)
    {
        _logBuilder.Clear();
    _messageCount = 0;
   LogTextBox.Text = "";
        LogCountText.Text = "0 messages";
  AddDebugLog("Log cleared", "INFO");
  }

    public void AddDebugLog(string message, string level = "DEBUG")
    {
   if (!Dispatcher.CheckAccess())
 {
   Dispatcher.Invoke(() => AddDebugLog(message, level));
          return;
        }

     var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
 var logEntry = $"[{timestamp}] [{level}] {message}\n";
   
  _logBuilder.Append(logEntry);
      _messageCount++;
    
   LogTextBox.Text = _logBuilder.ToString();
        LogCountText.Text = $"{_messageCount} messages";
        
        if (AutoScrollCheckBox.IsChecked == true)
        {
   LogScrollViewer.ScrollToEnd();
        }
    }

    private class DebugTraceListener : TraceListener
    {
        private readonly MainWindow _window;

 public DebugTraceListener(MainWindow window)
        {
          _window = window;
        }

    public override void Write(string? message)
 {
            if (!string.IsNullOrEmpty(message))
  {
   _window.AddDebugLog(message, "TRACE");
 }
  }

 public override void WriteLine(string? message)
   {
            if (!string.IsNullOrEmpty(message))
            {
       _window.AddDebugLog(message, "DEBUG");
}
   }
    }

    private void CardCheckTimer_Tick(object? sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_selectedReader))
        {
            CheckCardPresence();
        }
    }

    private void ReaderComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
 if (ReaderComboBox.SelectedItem is string readerName)
        {
    _selectedReader = readerName;

    var readerType = _nfcService.GetReaderType(readerName);
   ReaderTypeText.Text = readerType;

  ConnectButton.IsEnabled = true;
  UpdateConnectionStatus(false);
      StatusText.Text = "Place card on reader to start";
         WriteButton.IsEnabled = false;
      ReadButton.IsEnabled = false;
_cardCheckTimer?.Stop();
   }
    }

    private void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
   if (string.IsNullOrEmpty(_selectedReader))
        {
    MessageBox.Show("Please select a reader", "Warning",
      MessageBoxButton.OK, MessageBoxImage.Warning);
    return;
   }

 Debug.WriteLine($"UI: Connect button clicked for reader: {_selectedReader}");
 ConnectButton.IsEnabled = false;
    StatusText.Text = "Connecting to reader...";

        Task.Run(() =>
      {
    var isConnected = _nfcService.ConnectToReader(_selectedReader!);

   Dispatcher.Invoke(() =>
            {
     ConnectButton.IsEnabled = true;

       if (isConnected)
       {
  Debug.WriteLine("UI: Connection successful");
  UpdateConnectionStatus(true);
         StatusText.Text = "Reader connected and card detected!";
      ReadButton.IsEnabled = true;
          WriteButton.IsEnabled = true;

       _cardCheckTimer?.Start();
  ReadCardUid();
     }
     else
       {
      Debug.WriteLine("UI: Connection failed - no card or connection error");
     UpdateConnectionStatus(false);
  StatusText.Text = "No card - place card and try again";
       ReadButton.IsEnabled = false;
        WriteButton.IsEnabled = false;
         }
        });
        });
    }

    private void CheckCardPresence()
    {
 if (string.IsNullOrEmpty(_selectedReader))
    {
         return;
    }

        Task.Run(() =>
     {
     var isPresent = _nfcService.IsCardPresent(_selectedReader);

      Dispatcher.Invoke(() =>
  {
  if (isPresent && !_isCardPresent)
 {
       _isCardPresent = true;
      CardIndicator.Fill = new SolidColorBrush(Color.FromRgb(0, 120, 212));
       CardStatusText.Text = "Card detected";
     CardStatusText.Foreground = new SolidColorBrush(Color.FromRgb(0, 120, 212));
 StatusText.Text = "Card present. Reading UID...";
    ReadButton.IsEnabled = true;
         WriteButton.IsEnabled = true;
       ReadCardUid();
       }
     else if (!isPresent && _isCardPresent)
          {
        _isCardPresent = false;
                 CardIndicator.Fill = new SolidColorBrush(Color.FromRgb(164, 164, 164));
   CardStatusText.Text = "No card";
        CardStatusText.Foreground = new SolidColorBrush(Color.FromRgb(96, 94, 92));
  StatusText.Text = "Place card on reader";
      CardUidText.Visibility = Visibility.Collapsed;
          CardUidText.Text = "";
   ReadButton.IsEnabled = false;
          WriteButton.IsEnabled = false;
      }
      });
        });
    }

    private void ReadCardUid()
    {
        if (string.IsNullOrEmpty(_selectedReader))
  {
            return;
        }

      Task.Run(() =>
 {
            var uid = _nfcService.ReadCardUid(_selectedReader);

            Dispatcher.Invoke(() =>
        {
      if (uid != null && uid.Length > 0)
  {
    var uidHex = BitConverter.ToString(uid).Replace("-", " ");
        CardUidText.Text = $"UID: {uidHex}";
         CardUidText.Visibility = Visibility.Visible;
    StatusText.Text = "Ready for read/write";
     }
        else
  {
       StatusText.Text = "Card detected but cannot read UID";
       }
      });
        });
    }

    private void UpdateConnectionStatus(bool isConnected)
    {
   if (isConnected)
        {
     ConnectionIndicator.Fill = new SolidColorBrush(Color.FromRgb(0, 200, 83));
  ConnectionStatusText.Text = "Connected";
    ConnectionStatusText.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 83));
        }
        else
        {
 ConnectionIndicator.Fill = new SolidColorBrush(Color.FromRgb(164, 164, 164));
      ConnectionStatusText.Text = "Not connected";
            ConnectionStatusText.Foreground = new SolidColorBrush(Color.FromRgb(96, 94, 92));
        }
    }

    private void OnCardInserted(object? sender, string readerName)
    {
        Dispatcher.Invoke(() =>
        {
    if (_selectedReader == readerName)
            {
                StatusText.Text = "Card detected. Connecting...";

 Task.Run(() =>
     {
          var isConnected = _nfcService.ConnectToReader(readerName);

            Dispatcher.Invoke(() =>
           {
   if (isConnected)
        {
  _isCardPresent = true;
     CardIndicator.Fill = new SolidColorBrush(Color.FromRgb(0, 120, 212));
        CardStatusText.Text = "Card detected and connected";
   CardStatusText.Foreground = new SolidColorBrush(Color.FromRgb(0, 120, 212));
        UpdateConnectionStatus(true);
       StatusText.Text = "Ready! Click 'Read' to read card data.";
           ReadButton.IsEnabled = true;
              WriteButton.IsEnabled = true;

                   var uid = _nfcService.ReadCardUid(readerName);
    if (uid != null && uid.Length > 0)
 {
        var uidHex = BitConverter.ToString(uid).Replace("-", " ");
        CardUidText.Text = $"UID: {uidHex}";
      CardUidText.Visibility = Visibility.Visible;
     }
    }
            else
         {
          StatusText.Text = "Error during automatic connection";
          }
     });
    });
            }
        });
    }

    private void OnCardRemoved(object? sender, string readerName)
    {
        Dispatcher.Invoke(() =>
   {
            _isCardPresent = false;
   CardIndicator.Fill = new SolidColorBrush(Color.FromRgb(164, 164, 164));
       CardStatusText.Text = "No card";
            CardStatusText.Foreground = new SolidColorBrush(Color.FromRgb(96, 94, 92));
     StatusText.Text = "Place card on reader";
    CardUidText.Visibility = Visibility.Collapsed;
       CardUidText.Text = "";
       UpdateConnectionStatus(false);
 ReadButton.IsEnabled = false;
            WriteButton.IsEnabled = false;
        });
    }

    private async void WriteButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_selectedReader))
        {
            MessageBox.Show("Please select a reader", "Warning",
    MessageBoxButton.OK, MessageBoxImage.Warning);
  return;
        }

   if (!_isCardPresent)
        {
          MessageBox.Show("Please place a card on the reader", "Warning",
      MessageBoxButton.OK, MessageBoxImage.Warning);
  return;
        }

        WriteButton.IsEnabled = false;
        StatusText.Text = "Reading card...";

        var cardInfo = await Task.Run(() => _nfcService.ReadAllCardData(_selectedReader));

WriteButton.IsEnabled = true;

        if (cardInfo == null)
 {
            StatusText.Text = "Ready for next operation";
            MessageBox.Show("Failed to read card information.\n\nPlease try again.",
          "Error", MessageBoxButton.OK, MessageBoxImage.Error);
     return;
        }

    var writeWindow = new WriteGuidWindow(_nfcService, _selectedReader, cardInfo)
{
            Owner = this
   };

        var result = writeWindow.ShowDialog();

if (result == true && !string.IsNullOrEmpty(writeWindow.WrittenGuid))
        {
            StatusText.Text = "GUID written successfully!";
   await Task.Delay(1000);
      StatusText.Text = "Ready for next operation";
        }
     else
     {
            StatusText.Text = "Ready for next operation";
 }
    }

    private void ReadButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_selectedReader))
        {
            MessageBox.Show("Please select a reader", "Warning",
          MessageBoxButton.OK, MessageBoxImage.Warning);
         return;
        }

        if (!_isCardPresent)
        {
    MessageBox.Show("Please place a card on the reader", "Warning",
   MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
        }

        ReadButton.IsEnabled = false;
 StatusText.Text = "Reading...";

   Task.Run(() =>
        {
var cardInfo = _nfcService.ReadAllCardData(_selectedReader!);
   var userData = _nfcService.ReadUserDataFromCard(_selectedReader!);

 Dispatcher.Invoke(() =>
            {
       ReadButton.IsEnabled = true;

         if (cardInfo != null)
          {
 StatusText.Text = "Read successful!";
CardUidText.Text = $"UID: {cardInfo.UID} | Type: {cardInfo.CardType}";
        CardUidText.Visibility = Visibility.Visible;

            var resultWindow = new ReadResultWindow(cardInfo, userData)
         {
           Owner = this
      };
 resultWindow.ShowDialog();

    Task.Delay(2000).ContinueWith(_ =>
            {
      Dispatcher.Invoke(() =>
   {
    StatusText.Text = "Ready for next operation";
   });
});
         }
     else
          {
   StatusText.Text = "Error reading card";
 MessageBox.Show("Error reading card data.",
      "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
     });
        });
    }
}
