namespace NFC_card_tool;

public class CardInfo
{
    public string UID { get; set; } = string.Empty;
    public int UIDLength { get; set; }
    public string ATR { get; set; } = string.Empty;
    public string CardType { get; set; } = "Unknown";
    public string Protocol { get; set; } = string.Empty;
    public string MemorySize { get; set; } = string.Empty;
    public string? NDEFData { get; set; }
    public bool HasNDEF { get; set; }
    public string? ManufacturerData { get; set; }
    public bool IsFactoryState { get; set; }
    public Dictionary<int, string>? BlockData { get; set; }
    public int UsedBlocks { get; set; }
    public string? CardVersion { get; set; }
    public bool IsLocked { get; set; }

    public string GetFormattedInfo()
    {
        var info = new System.Text.StringBuilder();

        info.AppendLine("NFC CARD COMPREHENSIVE ANALYSIS");
        info.AppendLine("================================================");
        info.AppendLine();

        // IDENTIFICATION
        info.AppendLine("IDENTIFICATION");
        info.AppendLine($"  UID: {UID}");
        info.AppendLine($"  UID Length: {UIDLength} bytes ({GetUIDType(UIDLength)})");
        info.AppendLine($"  Card Type: {CardType}");
        if (!string.IsNullOrEmpty(CardVersion))
        {
            info.AppendLine($"  Version: {CardVersion}");
        }
        info.AppendLine();

        // TECHNICAL DATA
        info.AppendLine("TECHNICAL SPECIFICATIONS");
        info.AppendLine($"  ATR: {ATR}");
        info.AppendLine($"  Protocol: {Protocol}");
        if (!string.IsNullOrEmpty(MemorySize))
        {
            info.AppendLine($"  Memory: {MemorySize}");
        }
        if (!string.IsNullOrEmpty(ManufacturerData))
        {
            info.AppendLine($"  Manufacturer Block: {ManufacturerData}");
        }
        info.AppendLine();

        // CARD STATUS
        info.AppendLine("CARD STATUS");
        info.AppendLine($"  Factory State: {(IsFactoryState ? "YES - Card is blank/unused" : "NO - Card contains user data")}");
        info.AppendLine($"  Lock Status: {(IsLocked ? "LOCKED - Write protected" : "UNLOCKED - Writable")}");
        info.AppendLine($"  NDEF Support: {(HasNDEF ? "YES" : "NO")}");
        if (BlockData != null)
        {
            info.AppendLine($"  Readable Blocks: {BlockData.Count} blocks");
            info.AppendLine($"  Used Blocks: {UsedBlocks} blocks with data");
            info.AppendLine($"  Empty Blocks: {BlockData.Count - UsedBlocks - 1} blocks"); // -1 for manufacturer block
        }
        info.AppendLine();

        // DATA CONTENT
        if (HasNDEF && !string.IsNullOrEmpty(NDEFData))
        {
            info.AppendLine("NDEF DATA");
            info.AppendLine($"  {NDEFData}");
            info.AppendLine();
        }

        // BLOCK DATA (if has user data)
        if (!IsFactoryState && BlockData != null && BlockData.Count > 0)
        {
            info.AppendLine("MEMORY DUMP (Non-Empty Blocks)");
            foreach (var block in BlockData.OrderBy(b => b.Key))
            {
                // Skip empty blocks
                if (block.Key == 0)
                {
                    continue; // Skip manufacturer block
                }

                var bytes = block.Value.Split(' ');
                bool hasData = bytes.Any(b => b != "00" && b != "FF");

                if (hasData)
                {
                    info.AppendLine($"  Block {block.Key:D3}: {block.Value}");
                }
            }
            info.AppendLine();
        }

        // SUMMARY
        info.AppendLine("SUMMARY");
        if (IsFactoryState)
        {
            info.AppendLine("  This card is in FACTORY STATE (blank/new)");
            info.AppendLine("  No user data has been written to the card");
        }
        else
        {
            info.AppendLine("  This card contains USER DATA");
            info.AppendLine($"  {UsedBlocks} blocks are being used");
            if (HasNDEF)
            {
                info.AppendLine("  Card contains NDEF formatted data");
            }
        }

        info.AppendLine();
        info.AppendLine("================================================");

        return info.ToString();
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
}
