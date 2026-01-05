using System;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Configuration;

public class BillCasseteConfiguration
{
    public int CassetteNumber { get; private set; }
    public BillDenomination BillDenomination { get; set; }
    public BillCassetteDenominationMagnetStatus? DenominationMagnetStatus { get; set; }

    public BillCasseteConfiguration(int cassetteNumber, BillDenomination billDenomination, BillCassetteDenominationMagnetStatus? denominationMagnetStatus = null)
    {
        CassetteNumber = cassetteNumber;
        BillDenomination = billDenomination;
        DenominationMagnetStatus = denominationMagnetStatus;
    }

    public void SetBillDenomination(BillDenomination billDenomination)
    {
        BillDenomination = billDenomination;
        // TODO: Dispatch event that reset of bill dispenser is necessary
    }

    public void SetDenominationMagnetStatus(BillCassetteDenominationMagnetStatus denominationMagnetStatus)
    {
        DenominationMagnetStatus = denominationMagnetStatus;
        // TODO: Dispatch event that reset of bill dispenser is necessary
    }
}

public class BillDenomination
{
    // Properties for Length, Width, and Thickness in millimeters
    public int Width { get; init; }
    public int Length { get; init; }
    public int Thickness { get; init; }
    public int Value { get; init; }
    public Currency Currency { get; init; }

    public BillDenomination(int value, Currency currency, int width, int length, int thickness)
    {
        Width = width;
        Length = length;
        Thickness = thickness;
        Value = value;
        Currency = currency;
    }
}

public class BillCassetteDenominationMagnetStatus
{
    public bool MagnetA { get; init; }
    public bool MagnetB { get; init; }
    public bool MagnetC { get; init; }
    public bool MagnetD { get; init; }

    public BillCassetteDenominationMagnetStatus() { }

    public BillCassetteDenominationMagnetStatus(bool magnetA = false, bool magnetB = false, bool magnetC = false, bool magnetD = false)
    {
        MagnetA = magnetA;
        MagnetB = magnetB;
        MagnetC = magnetC;
        MagnetD = magnetD;
    }

    /// <summary>
    /// Generates a hash code for the current object based on the Magnet status properties.
    /// </summary>
    /// <returns>A hash code that represents the combination of the MagnetA, MagnetB, MagnetC, and MagnetD values.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(MagnetA, MagnetB, MagnetC, MagnetD);
    }
}