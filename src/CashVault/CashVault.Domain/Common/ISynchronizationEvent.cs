namespace CashVault.Application.Common
{
    /// <summary>
    /// Interface used to ensure that events implementing it cannot be executed concurrently.
    /// </summary>
    public interface ISynchronizationEvent
    {
    }
}
