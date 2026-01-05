using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;

namespace CashVault.ParcelLockerDriver.Config;

public class ParcelLockerConfiguration : IParcelLockerConfiguration
{
    public List<int> SupportedBaudRates { get; set; } = [9600, 14400, 19200, 38400, 57600, 115200];

    public byte BoardAddress { get; set; } = 0x01;

    public int BaudRate { get; set; } = 9600;

    public int CabinetCount { get; set; } = 24;

    public bool EnablePeriodicStatusCheck { get; set; } = true;

    public int StatusCheckIntervalSeconds { get; set; } = 5;

    public bool IsEnabled { get; set; } = false;

    public List<ParcelLocker> ParcelLockers { get; set; } = [];

    public ParcelLockerConfiguration()
    {
        BoardAddress = 0x01;
        BaudRate = 9600;
        CabinetCount = 24;
        EnablePeriodicStatusCheck = true;
        StatusCheckIntervalSeconds = 5;
        IsEnabled = false;
        ParcelLockers = [];

        // TODO: Remove this
        if (ParcelLockers is null || ParcelLockers.Count == 0)
        {
            /*
            ParcelLockers =
            [
                // no assigned postal service
                new(1, "PL-001", ParcelLockerSize.Small),
                new(2, "PL-002", ParcelLockerSize.Medium),
                new(3, "PL-003", ParcelLockerSize.Large),
                new(4, "PL-004", ParcelLockerSize.Small),
                new(5, "PL-005", ParcelLockerSize.Medium),
                new(6, "PL-006", ParcelLockerSize.Large),

                // XExpress lockers
                new(7, "PL-007", ParcelLockerSize.Small, "XExpress"),
                new(8, "PL-008", ParcelLockerSize.Medium, "XExpress"),
                new(9, "PL-009", ParcelLockerSize.Large, "XExpress"),

                // EuroExpress lockers
                new(10, "PL-010", ParcelLockerSize.Small, "EuroExpress"),
                new(11, "PL-011", ParcelLockerSize.Medium, "EuroExpress"),
                new(12, "PL-012", ParcelLockerSize.Large, "EuroExpress"),

                // A2B lockers
                new(13, "PL-013", ParcelLockerSize.Small, "A2B"),
                new(14, "PL-014", ParcelLockerSize.Medium, "A2B"),
                new(15, "PL-015", ParcelLockerSize.Large, "A2B"),
            ];
            */

            ParcelLockers = new(CabinetCount);

            for (int i = 1; i <= CabinetCount; ++i)
            {
                ParcelLockerSize size = ParcelLockerSize.Small;
                if (i % 3 == 1)
                    size = ParcelLockerSize.Medium;
                else if (i % 3 == 2)
                    size = ParcelLockerSize.Large;

                ParcelLockers.Add(new(i, $"PL-{i}", size));
            }

        }
    }

    public void Validate()
    {
        if (!SupportedBaudRates.Contains(BaudRate))
            throw new InvalidDataException("Baud rate is not supported.");
    }

    public ParcelLocker? GetLocker(int id)
    {
        if (ParcelLockers is null || ParcelLockers.Count == 0)
            return null;

        return ParcelLockers.FirstOrDefault(x => x.Id == id);
    }

    public ParcelLocker? GetLocker(Func<ParcelLocker, bool> predicate)
    {
        if (ParcelLockers is null || ParcelLockers.Count == 0)
            return null;

        return ParcelLockers.FirstOrDefault(predicate);
    }

    public ParcelLocker? GetAvailableLocker(string postalService, string size)
    {
        if (ParcelLockers is null || ParcelLockers.Count == 0)
            return null;

        List<ParcelLocker> availableLockers = ParcelLockers.Where(locker =>
                locker.IsActive && locker.IsEmpty && locker.Size.Code.Equals(size) &&
                (string.IsNullOrWhiteSpace(locker.PostalService) || postalService.Equals(locker.PostalService)))
            .ToList();

        return availableLockers.OrderBy(x => string.IsNullOrWhiteSpace(x.PostalService))
            .FirstOrDefault();
    }
}
