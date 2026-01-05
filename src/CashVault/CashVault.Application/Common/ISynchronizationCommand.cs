namespace CashVault.Application.Common
{
    /// <summary>
    /// Interface used to ensure that commands implementing it cannot be executed concurrently.
    /// </summary>
    public interface ISynchronizationCommand
    {
    }
}
