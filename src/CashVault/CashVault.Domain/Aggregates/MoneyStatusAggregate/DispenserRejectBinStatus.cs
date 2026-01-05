using System.Collections.Generic;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate;

public class DispenserRejectBinStatus
{
    public int BillCount { get; set; }
    public List<RejectBinSource> Sources { get; set; } = [];

    public DispenserRejectBinStatus()
    {
        BillCount = 0;
        Sources = [];
    }

    public void AddSource(RejectBinSource source)
    {
        if (source == null || source.Count <= 0) return;

        var existingSource = Sources.Find(s => s.CassetteNumber == source.CassetteNumber);

        if (existingSource != null)
        {
            existingSource.Count += source.Count;
        }
        else
        {
            Sources.Add(source);
        }

        BillCount += source.Count;
    }

    public void Clear()
    {
        BillCount = 0;
        Sources.Clear();
    }
}

public class RejectBinSource
{
    public int CassetteNumber { get; set; }
    public int Denom { get; set; }
    public int Count { get; set; }

    public RejectBinSource(int cassetteNumber, int denom, int count)
    {
        CassetteNumber = cassetteNumber;
        Denom = denom;
        Count = count;
    }
}
