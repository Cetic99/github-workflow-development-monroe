using PCSC;
using PCSC.Monitoring;
using PCSC.Exceptions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace NFC_card_tool;

public class NfcReaderService : IDisposable
{
    private ISCardContext? _context;
    private ISCardMonitor? _monitor;

    public ObservableCollection<string> AvailableReaders { get; } = new();
    public event EventHandler<string>? CardInserted;
    public event EventHandler<string>? CardRemoved;

    public void Initialize()
    {
        try
        {
            Debug.WriteLine("Initializing NFC service...");
            _context = ContextFactory.Instance.Establish(SCardScope.System);
            Debug.WriteLine("PC/SC context established successfully");
            RefreshReaders();
            StartMonitoring();
            Debug.WriteLine($"NFC service initialized. Found {AvailableReaders.Count} readers");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR: Failed to initialize NFC service: {ex.Message}");
            throw new InvalidOperationException($"Error initializing NFC service: {ex.Message}", ex);
        }
    }

    public void RefreshReaders()
    {
        if (_context == null) return;
        AvailableReaders.Clear();
  
   try
        {
            var readerNames = _context.GetReaders();
    if (readerNames != null && readerNames.Length > 0)
     {
       foreach (var reader in readerNames)
      {
  AvailableReaders.Add(reader);
   Debug.WriteLine($"Found reader: {reader}");
      }
 }
  else
   {
      Debug.WriteLine("GetReaders() returned null or empty array");
      }
  }
        catch (NoReadersAvailableException)
        {
    Debug.WriteLine("NoReadersAvailableException - no readers connected");
    // No readers available
        }
  catch (Exception ex)
   {
    Debug.WriteLine($"Error in RefreshReaders: {ex.Message}");
      }
    }

    private void StartMonitoring()
    {
        if (_context == null) return;
 
        try
        {
var readerNames = _context.GetReaders();
  if (readerNames == null || readerNames.Length == 0)
        {
    Debug.WriteLine("StartMonitoring: No readers to monitor");
return;
  }

   Debug.WriteLine($"StartMonitoring: Monitoring {readerNames.Length} readers");
 
   _monitor = MonitorFactory.Instance.Create(SCardScope.System);
    _monitor.CardInserted += (s, e) => CardInserted?.Invoke(this, e.ReaderName);
    _monitor.CardRemoved += (s, e) => CardRemoved?.Invoke(this, e.ReaderName);
_monitor.Start(readerNames);
      
  Debug.WriteLine("Monitoring started successfully");
        }
        catch (Exception ex)
        {
    Debug.WriteLine($"StartMonitoring error: {ex.Message}");
     // Monitoring failed
        }
    }

    public string GetReaderType(string readerName)
    {
        if (readerName.Contains("ACR1252", StringComparison.OrdinalIgnoreCase)) return "ACR1252U";
        if (readerName.Contains("ACR122", StringComparison.OrdinalIgnoreCase)) return "ACR122U";
        if (readerName.Contains("ACS", StringComparison.OrdinalIgnoreCase)) return "ACS Reader";
        return "Unknown";
    }

    public bool ConnectToReader(string readerName)
    {
        if (_context == null) return false;
   
   Debug.WriteLine($"Attempting to connect to reader: {readerName}");
        try
    {
    var readerState = new SCardReaderState { ReaderName = readerName, CurrentState = SCRState.Unaware };
            var readerStates = new[] { readerState };
  _context.GetStatusChange(IntPtr.Zero, readerStates);
    
         var state = readerStates[0].EventState;
        var hasCard = (state & SCRState.Present) != 0;
  
            Debug.WriteLine($"Reader state: {state}, Card present: {hasCard}");
      
            if (!hasCard)
   {
        Debug.WriteLine("No card present on reader");
     return false;
  }

       var protocols = new[] { SCardProtocol.Any, SCardProtocol.T0, SCardProtocol.T1 };
     foreach (var protocol in protocols)
    {
try
     {
     using var reader = _context.ConnectReader(readerName, SCardShareMode.Shared, protocol);
         if (reader.IsConnected)
 {
         Debug.WriteLine($"Successfully connected using protocol: {protocol}");
       return true;
      }
       }
      catch { }
     }

   Debug.WriteLine("Failed to connect with any protocol");
     }
        catch (Exception ex)
        {
    Debug.WriteLine($"ERROR connecting to reader: {ex.Message}");
        }
     
        return false;
    }

    public bool IsCardPresent(string readerName)
 {
        if (_context == null) return false;
 
      try
        {
   var readerState = new SCardReaderState { ReaderName = readerName, CurrentState = SCRState.Unaware };
  var readerStates = new[] { readerState };
    var result = _context.GetStatusChange(IntPtr.Zero, readerStates);
      
     if (result == SCardError.Success)
         {
  return (readerStates[0].EventState & SCRState.Present) != 0;
            }
        }
  catch { }
        
   return false;
    }

    public byte[]? ReadCardUid(string readerName)
    {
        if (_context == null) return null;
      
        try
    {
     using var reader = _context.ConnectReader(readerName, SCardShareMode.Shared, SCardProtocol.Any);
    var apduGetUid = new byte[] { 0xFF, 0xCA, 0x00, 0x00, 0x00 };
      var responseBuffer = new byte[256];
    var bytesReceived = reader.Transmit(apduGetUid, responseBuffer);
 
Debug.WriteLine($"UID read: {bytesReceived} bytes received");
     
      if (bytesReceived >= 2 && responseBuffer[bytesReceived - 2] == 0x90 && responseBuffer[bytesReceived - 1] == 0x00)
    {
          var uid = new byte[bytesReceived - 2];
              Array.Copy(responseBuffer, uid, bytesReceived - 2);
    var uidHex = BitConverter.ToString(uid).Replace("-", " ");
  Debug.WriteLine($"Card UID: {uidHex}");
     return uid;
    }
     else
   {
      Debug.WriteLine("UID read failed - invalid response");
        }
    }
        catch (Exception ex)
        {
     Debug.WriteLine($"ERROR reading UID: {ex.Message}");
   }
   
  return null;
    }

    public CardInfo? ReadAllCardData(string readerName)
    {
   if (_context == null) return null;

     Debug.WriteLine($"Reading all card data from: {readerName}");
        try
     {
      using var reader = _context.ConnectReader(readerName, SCardShareMode.Shared, SCardProtocol.Any);
   var cardInfo = new CardInfo();

    // Read UID
      var apduGetUid = new byte[] { 0xFF, 0xCA, 0x00, 0x00, 0x00 };
      var responseBuffer = new byte[256];
    var bytesReceived = reader.Transmit(apduGetUid, responseBuffer);
  
      if (bytesReceived >= 2)
      {
        var sw1 = responseBuffer[bytesReceived - 2];
        var sw2 = responseBuffer[bytesReceived - 1];
   
       if (sw1 == 0x90 && sw2 == 0x00)
 {
     var uid = new byte[bytesReceived - 2];
           Array.Copy(responseBuffer, uid, bytesReceived - 2);
    cardInfo.UID = BitConverter.ToString(uid).Replace("-", " ");
        cardInfo.UIDLength = uid.Length;
      Debug.WriteLine($"UID: {cardInfo.UID}, Length: {cardInfo.UIDLength}");
       }
            }

    // Read ATR
       var atr = reader.GetAttrib(SCardAttribute.AtrString);
   if (atr != null && atr.Length > 0)
   {
  cardInfo.ATR = BitConverter.ToString(atr).Replace("-", " ");
  cardInfo.CardType = IdentifyCardType(atr);
    Debug.WriteLine($"ATR: {cardInfo.ATR}");
     Debug.WriteLine($"Card Type: {cardInfo.CardType}");
      }

    // Protocol
      cardInfo.Protocol = reader.Protocol.ToString();
   Debug.WriteLine($"Protocol: {cardInfo.Protocol}");

     // Memory analysis for NTAG
      if (cardInfo.CardType.Contains("NTAG"))
         {
    Debug.WriteLine("Reading NTAG memory...");
       cardInfo.MemorySize = GetNtagMemorySize(cardInfo.CardType);
  cardInfo.BlockData = ReadNtagPages(reader, cardInfo.CardType);
     cardInfo.UsedBlocks = CountUsedBlocks(cardInfo.BlockData);
  cardInfo.IsFactoryState = CheckNtagFactoryState(cardInfo.BlockData);
       Debug.WriteLine($"Memory Size: {cardInfo.MemorySize}, Used Blocks: {cardInfo.UsedBlocks}, Factory State: {cardInfo.IsFactoryState}");
 }

   Debug.WriteLine("Card data read successfully");
   return cardInfo;
        }
        catch (Exception ex)
      {
   Debug.WriteLine($"ERROR reading card data: {ex.Message}");
   return null;
   }
    }

    private string IdentifyCardType(byte[] atr)
    {
     var atrHex = BitConverter.ToString(atr).Replace("-", "");

        // NTAG215 exact match
        if (atrHex.Equals("3B8F8001804F0CA0000003060300030000000068", StringComparison.OrdinalIgnoreCase))
     {
            return "NTAG215 (504 bytes)";
    }

        // Generic NTAG patterns
        if (atrHex.Contains("8F8001") && atrHex.Contains("4F0C"))
        {
     if (atrHex.Contains("030003")) return "NTAG215 (504 bytes)";
        if (atrHex.Contains("030001")) return "NTAG213 (144 bytes)";
            if (atrHex.Contains("030005")) return "NTAG216 (888 bytes)";
        }

    // Fallback
        if (atrHex.Contains("8F80")) return "NTAG (unknown variant)";
    return "ISO 14443 Type A";
    }

    private string GetNtagMemorySize(string cardType)
    {
        if (cardType.Contains("NTAG213")) return "144 bytes (user memory)";
   if (cardType.Contains("NTAG215")) return "504 bytes (user memory)";
        if (cardType.Contains("NTAG216")) return "888 bytes (user memory)";
        return "Unknown NTAG variant";
    }

    private Dictionary<int, string> ReadNtagPages(ICardReader reader, string cardType)
    {
  var pages = new Dictionary<int, string>();
        int maxPage = 39;
    
        if (cardType.Contains("NTAG215")) maxPage = 129;
        else if (cardType.Contains("NTAG216")) maxPage = 225;

        // Start from page 4 (pages 0-3 are factory locked!)
        for (int page = 4; page <= maxPage; page++)
    {
            try
        {
         var readCmd = new byte[] { 0xFF, 0xB0, 0x00, (byte)page, 0x04 };
          var response = new byte[256];
         var bytes = reader.Transmit(readCmd, response);

              if (bytes >= 6 && response[bytes - 2] == 0x90)
     {
           var pageData = new byte[4];
    Array.Copy(response, pageData, 4);
  pages[page] = BitConverter.ToString(pageData).Replace("-", " ");
         }
            }
   catch { continue; }
        }

     return pages;
    }

    private bool CheckNtagFactoryState(Dictionary<int, string> pageData)
{
        try
        {
            int emptyPages = 0;
       int totalChecked = 0;

       // Check pages 4-15
   for (int page = 4; page <= 15; page++)
     {
     if (pageData.ContainsKey(page))
       {
      totalChecked++;
          var bytes = pageData[page].Split(' ');

    bool isEmpty = true;
            int nonZeroCount = 0;
           int nonFFCount = 0;
      int printableCount = 0;

foreach (var byteStr in bytes)
         {
        if (byte.TryParse(byteStr, System.Globalization.NumberStyles.HexNumber, null, out byte b))
     {
      if (b != 0x00) nonZeroCount++;
  if (b != 0xFF) nonFFCount++;
      if (b >= 0x20 && b <= 0x7E) printableCount++;
      }
    }

          if (nonZeroCount == 0 || nonFFCount == 0 || printableCount < 2)
    {
              emptyPages++;
     }
        }
         }

        bool isFactory = totalChecked > 0 && ((double)emptyPages / totalChecked) >= 0.8;
         return isFactory;
        }
        catch (Exception)
        {
        return false;
        }
    }

    private int CountUsedBlocks(Dictionary<int, string> blockData)
    {
        int usedBlocks = 0;
    
        foreach (var block in blockData)
        {
    if (block.Key < 4) continue; // Skip pages 0-3
 
var bytes = block.Value.Split(' ');
   bool hasData = bytes.Any(b => b != "00" && b != "FF");
 
            if (hasData) usedBlocks++;
        }
        
        return usedBlocks;
    }

    // WRITE TO CARD - Start from page 4!
    public bool WriteDataToCard(string readerName, string data)
    {
        if (_context == null || string.IsNullOrEmpty(data)) return false;
 
        try
        {
     using var reader = _context.ConnectReader(readerName, SCardShareMode.Shared, SCardProtocol.Any);

            var dataBytes = System.Text.Encoding.ASCII.GetBytes(data);
          var atr = reader.GetAttrib(SCardAttribute.AtrString);
       var cardType = atr != null ? IdentifyCardType(atr) : "Unknown";

int startPage = 4; // Always start from page 4!
 int bytesPerPage = 4;
            int pagesNeeded = (dataBytes.Length + bytesPerPage - 1) / bytesPerPage;

            int bytesWritten = 0;
          for (int pageOffset = 0; pageOffset < pagesNeeded && bytesWritten < dataBytes.Length; pageOffset++)
 {
       int currentPage = startPage + pageOffset;
         byte[] pageData = new byte[bytesPerPage];
                int bytesToCopy = Math.Min(bytesPerPage, dataBytes.Length - bytesWritten);
      
   if (bytesToCopy > 0)
              {
         Array.Copy(dataBytes, bytesWritten, pageData, 0, bytesToCopy);
            }

    bool success = WriteNtagPage(reader, currentPage, pageData);
         if (success)
          {
         bytesWritten += bytesToCopy;
    }
  else
         {
            return false;
    }
            }

         return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    // SAFE WRITE - Don't write to factory locked pages 0-3
    private bool WriteNtagPage(ICardReader reader, int pageNumber, byte[] data)
    {
  try
        {
            // Safety check: Block pages 0-3 (factory locked)
          if (pageNumber < 4)
        {
   return false;
 }

       byte[] writeData = new byte[4];
  Array.Copy(data, writeData, Math.Min(4, data.Length));

  var updateCmd = new byte[] { 0xFF, 0xD6, 0x00, (byte)pageNumber, 0x04 }
                .Concat(writeData).ToArray();

            var response = new byte[256];
            var bytes = reader.Transmit(updateCmd, response);

bool success = bytes >= 2 && response[bytes - 2] == 0x90 && response[bytes - 1] == 0x00;
     return success;
        }
catch (Exception)
    {
         return false;
        }
    }

    // READ USER DATA - Start from page 4!
    public string? ReadUserDataFromCard(string readerName)
    {
        if (_context == null) return null;
        
        try
  {
            using var reader = _context.ConnectReader(readerName, SCardShareMode.Shared, SCardProtocol.Any);

         var atr = reader.GetAttrib(SCardAttribute.AtrString);
    var cardType = atr != null ? IdentifyCardType(atr) : "Unknown";

          var userDataBytes = new List<byte>();
            int startPage = 4; // Start from page 4!
            int pagesToRead = 12; // 12 pages = 48 bytes

       for (int page = startPage; page < startPage + pagesToRead; page++)
            {
var readCmd = new byte[] { 0xFF, 0xB0, 0x00, (byte)page, 0x04 };
      var response = new byte[256];
         var bytes = reader.Transmit(readCmd, response);

        if (bytes >= 6 && response[bytes - 2] == 0x90)
        {
           var pageData = new byte[4];
  Array.Copy(response, pageData, 4);
        userDataBytes.AddRange(pageData);
   }
            }

 // Check if card is factory empty
        int nonZeroBytes = 0;
          int nonFFBytes = 0;
            int printableBytes = 0;

            for (int i = 0; i < userDataBytes.Count; i++)
{
           byte b = userDataBytes[i];
                if (b != 0x00) nonZeroBytes++;
           if (b != 0xFF) nonFFBytes++;

                if ((b >= 0x20 && b <= 0x7E) || b == 0x09 || b == 0x0A || b == 0x0D)
      {
          printableBytes++;
         }
 }

            // Factory card detection
  if (nonZeroBytes == 0 || nonFFBytes == 0 || nonZeroBytes < 5 && nonFFBytes < 5)
    {
    return null;
   }

    double printableRatio = (double)printableBytes / userDataBytes.Count;
     if (printableRatio < 0.10)
            {
  return null;
   }

     // Find end of actual data
 int dataEnd = userDataBytes.Count;
   for (int i = 0; i < userDataBytes.Count; i++)
       {
  if (userDataBytes[i] == 0x00 || userDataBytes[i] == 0xFF)
          {
        bool restIsEmpty = true;
  for (int j = i; j < userDataBytes.Count; j++)
         {
  if (userDataBytes[j] != 0x00 && userDataBytes[j] != 0xFF)
       {
    restIsEmpty = false;
           break;
   }
        }
    
  if (restIsEmpty)
              {
            dataEnd = i;
            break;
            }
 }
 }

       var actualData = userDataBytes.Take(dataEnd).ToArray();

    if (actualData.Length == 0)
   {
    return null;
            }

       var hexString = BitConverter.ToString(actualData).Replace("-", " ");
     return hexString;
      }
  catch (Exception)
   {
  return null;
        }
    }

    public void Dispose()
    {
        try
        {
     _monitor?.Cancel();
_monitor?.Dispose();
  _context?.Dispose();
        }
        catch (Exception)
        {
       // Ignore dispose errors
        }
    }
}
