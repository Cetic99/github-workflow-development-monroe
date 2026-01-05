
namespace CashVault.BillDispenserDriver.JCM.F53.Messages.Common;

internal class DispenserError
{
    public string? Code { get; init; }
    public string? Description { get; init; }
    public bool IsCritical { get; init; }
    public bool ResetNeeded { get; init; }
    public bool MaintenanceNeeded { get; init; }
}