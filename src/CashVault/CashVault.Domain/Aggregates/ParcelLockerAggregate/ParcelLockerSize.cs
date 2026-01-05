using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.ParcelLockerAggregate;

public class ParcelLockerSize : Enumeration
{
    public static readonly ParcelLockerSize ExtraSmall = new(1, "XS", nameof(ExtraSmall), 23m, 4m, 40m);
    public static readonly ParcelLockerSize Small = new(2, "S", nameof(Small), 38m, 8m, 64m);
    public static readonly ParcelLockerSize Medium = new(3, "M", nameof(Medium), 38m, 19m, 64m);
    public static readonly ParcelLockerSize Large = new(4, "L", nameof(Large), 38m, 41m, 64m);

    public string Name { get; private set; }
    public decimal Width { get; private set; }
    public decimal Height { get; private set; }
    public decimal Length { get; private set; }

    public string DimensionDisplayName
    {
        get => $"W{Width}xD{Length}xH{Height}";
    }

    public ParcelLockerSize(
        int id,
        string code,
        string name,
        decimal width,
        decimal height,
        decimal length)
        : base(id, code)
    {
        Name = name;
        Width = width;
        Height = height;
        Length = length;
    }
}
