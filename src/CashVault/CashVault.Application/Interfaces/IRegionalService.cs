using CashVault.Application.Common.Models;

namespace CashVault.Application.Interfaces
{
    public interface IRegionalService
    {
        /// <summary>
        /// Retrieves all available system time zones.
        /// </summary>
        /// <returns>A list of <see cref="TimeZoneModel"/> objects representing system time zones, including their display names, IDs, and GMT offsets.</returns>
        List<TimeZoneModel> GetAllTimeZones();

        /// <summary>
        /// Converts a UTC <see cref="DateTime"/> to a user's specified time zone.
        /// </summary>
        /// <param name="utcTime">The UTC <see cref="DateTime"/> to convert.</param>
        /// <param name="timeZoneId">The ID of the time zone to which the time will be converted. If null or empty, the current UTC time is returned.</param>
        /// <returns>The converted <see cref="DateTime"/> in the specified time zone, or the current UTC time if the time zone ID is null, empty, or invalid.</returns>
        DateTime ConvertToUserTimeZone(DateTime utcTime, string? timeZoneId);

        /// <summary>
        /// Retrieves the display name of a time zone by its ID.
        /// </summary>
        /// <param name="timeZoneId">The ID of the time zone.</param>
        /// <returns>The display name of the time zone.</returns>
        string GetUserTimeZoneDisplayName(string timeZoneId);

        /// <summary>
        /// Loades localization messages into cache
        /// </summary>
        Task<bool> LoadMessages();

        /// <summary>
        /// Loades localization messages into cache
        /// </summary>
        Task<bool> UpdateSingleMessage(string langCode, string key, string value);

        /// <summary>
        /// Retrieves a localized string
        /// </summary>
        /// <param name="key">Localization key</param>
        /// <returns>Localized string</returns>
        string Translate(string key);
    }
}
