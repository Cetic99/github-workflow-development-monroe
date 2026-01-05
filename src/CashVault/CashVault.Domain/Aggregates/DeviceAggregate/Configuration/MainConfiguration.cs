using System.Collections.Generic;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Configuration;

public class MainConfiguration
{
    public List<TerminalType> TerminalTypes { get; set; } = [];
    public string? DeviceName { get; set; }
    public DeviceModel? CabinetType { get; set; }
    public Port? CabinetInterface { get; set; }
    public DeviceModel? UserCardReaderType { get; set; }
    public Port? UserCardReaderInterface { get; set; }
    public DeviceModel? BillAcceptorType { get; set; }
    public Port? BillAcceptorInterface { get; set; }
    public DeviceModel? CoinAcceptorType { get; set; }
    public Port? CoinAcceptorInterface { get; set; }
    public DeviceModel? BillDispenserType { get; set; }
    public Port? BillDispenserInterface { get; set; }
    public DeviceModel? TITOPrinterType { get; set; }
    public Port? TITOPrinterInterface { get; set; }
    public DeviceModel? ParcelLockerType { get; set; }
    public Port? ParcelLockerInterface { get; set; }

    public MainConfiguration()
    {
        TerminalTypes = [];
        CabinetType = null;
        CabinetInterface = null;
        BillAcceptorType = null;
        BillAcceptorInterface = null;
        BillDispenserType = null;
        BillDispenserInterface = null;
        TITOPrinterType = null;
        TITOPrinterInterface = null;
        UserCardReaderType = null;
        UserCardReaderInterface = null;
        CoinAcceptorType = null;
        CoinAcceptorInterface = null;
        ParcelLockerType = null;
        ParcelLockerInterface = null;
    }

    public bool IsCabinetMainConfigUpdated()
    {
        if (CabinetType == null || CabinetInterface == null)
        {
            return false;
        }

        return true;
    }

    public bool IsUserCardReaderMainConfigUpdated()
    {
        if (UserCardReaderType == null || UserCardReaderInterface == null)
        {
            return false;
        }

        return true;
    }

    public bool IsBillDispenserMainConfigUpdated()
    {
        if (BillDispenserType == null || BillDispenserInterface == null)
        {
            return false;
        }

        return true;
    }

    public bool IsBillAcceptorMainConfigUpdated()
    {
        if (BillAcceptorType == null || BillAcceptorInterface == null)
        {
            return false;
        }

        return true;
    }

    public bool IsTITOPrinterMainConfigUpdated()
    {
        if (TITOPrinterType == null || TITOPrinterInterface == null)
        {
            return false;
        }

        return true;
    }

    public bool IsCoinAcceptorMainConfigUpdated()
    {
        if (CoinAcceptorType == null || CoinAcceptorInterface == null)
        {
            return false;
        }
        return true;
    }

    public bool IsParcelLockerMainConfigUpdated()
    {
        if (ParcelLockerType == null || ParcelLockerInterface == null)
        {
            return false;
        }

        return true;
    }
}