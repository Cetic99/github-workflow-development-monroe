using System.Globalization;
using CashVault.Application.Common.Models;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.MessageAggregate;
using CashVault.Domain.Common;
using Microsoft.Extensions.Caching.Memory;
using NodaTime;

namespace CashVault.Application.Services;

public class RegionalService : IRegionalService, ILocalizer
{
    private readonly ISessionService sessionService;
    private readonly IUnitOfWork unitOfWork;
    private readonly IMemoryCache memoryCache;
    private readonly string cacheKey = "Messages";

    public RegionalService(ISessionService sessionService, IMemoryCache memoryCache, IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
        this.sessionService = sessionService;
        this.memoryCache = memoryCache;
    }

    public List<TimeZoneModel> GetAllTimeZones()
    {
        return DateTimeZoneProviders.Tzdb.Ids
            .Select(id => new TimeZoneModel
            {
                Id = id,
                DisplayName = id
            }).ToList();
    }

    public DateTime ConvertToUserTimeZone(DateTime utcTime, string? timeZoneId)
    {
        if (string.IsNullOrEmpty(timeZoneId)) return utcTime;

        if (string.IsNullOrEmpty(timeZoneId) || timeZoneId == "---") return utcTime;

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

        if (timeZone == null) return utcTime;

        return TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZone);
    }

    public string GetUserTimeZoneDisplayName(string timeZoneId)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return timeZone.DisplayName;
    }

    public static string FormatNumber(decimal number, NumberSeparator decimalSeparatorConfig, NumberSeparator thousandSeparatorConfig)
    {
        string decimalSeparator = decimalSeparatorConfig.Code == NumberSeparator.Dot.Code ? "." : ",";
        string thousandSeparator = thousandSeparatorConfig.Code == NumberSeparator.Comma.Code ? "," : ".";

        string formattedNumber = number.ToString("#,0.00", CultureInfo.InvariantCulture);

        formattedNumber = formattedNumber.Replace(",", thousandSeparator).Replace(".", decimalSeparator);

        return formattedNumber;
    }

    public async Task<bool> LoadMessages()
    {
        var messages = await unitOfWork.MessageRepository.GetAllMessagesAsync();
        memoryCache.Set(cacheKey, messages ?? new List<Message>());

        return messages?.Count > 0;
    }

    public async Task<bool> UpdateSingleMessage(string langCode, string key, string value)
    {
        var messages = memoryCache.Get(cacheKey) as List<Message>;

        if (messages == null) return false;

        var cacheMessage =
            messages.FirstOrDefault
                (x => x.LanguageCode.Equals(langCode) &&
                      x.Key.Equals(key));

        if (cacheMessage == null) return false;

        cacheMessage.SetValue(value);
        cacheMessage.ClearDomainEvents();

        memoryCache.Set(cacheKey, messages ?? new List<Message>());

        return true;
    }

    public string Translate(string key)
    {
        var messages = memoryCache.Get(cacheKey) as List<Message>;
        var message = messages?.FirstOrDefault(x => x.Key.Equals(key) && x.LanguageCode == sessionService.Language);

        return message?.Value ?? key;
    }

    public string this[string key, params object[] args]
    {
        get
        {
            return string.Format(Translate(key), args);
        }
    }
}
