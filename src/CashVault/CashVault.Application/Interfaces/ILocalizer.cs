namespace CashVault.Application.Interfaces;

public interface ILocalizer
{
    /// <summary>
    /// Retrieves a localized string
    /// </summary>
    /// <param name="key">Localization key</param>
    /// <param name="args">Arguments to format the localized string</param>
    /// <returns>Localized string</returns>
    public string this[string key, params object[] args] { get; }
}
