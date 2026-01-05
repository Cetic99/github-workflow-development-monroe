using System.Text.Json;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CashVault.Infrastructure.PersistentStorage;

public class TerminalRepository : ITerminalRepository
{
    private readonly CashVaultContext _dbContext;
    private readonly IDeviceDriverFactory _driverFactory;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    public TerminalRepository(CashVaultContext dbContext, IDeviceDriverFactory driverFactory)
    {
        _dbContext = dbContext;
        _driverFactory = driverFactory;
    }

    private void UpdateConfiguration<T>(T config)
    {
        var item = _dbContext.Configurations.Where(x => x.Key == typeof(T).Name).FirstOrDefault();

        if (item == null)
        {
            item = new Configuration(typeof(T).Name, System.Text.Json.JsonSerializer.Serialize<T>(config));
            _dbContext.Configurations.Add(item);
        }
        else
        {
            item.Value = System.Text.Json.JsonSerializer.Serialize<T>(config);
        }
    }

    private async Task<T> GetConfiguration<T>()
    {
        var x = _driverFactory.MainConfiguration;
        var item = await _dbContext.Configurations.Where(x => x.Key == typeof(T).Name).FirstOrDefaultAsync();

        if (item == null) return default;

        var config = System.Text.Json.JsonSerializer.Deserialize<T>(item.Value);

        return await Task.FromResult(config);
    }

    private async Task<IBasicHardwareDeviceConfiguration?> GetDriverConfiguration<T>(DeviceType deviceType)
    {
        var item = await _dbContext.Configurations.Where(x => x.Key == typeof(T).Name).FirstOrDefaultAsync();

        return _driverFactory.CreateConfiguration(deviceType, item?.GetJson());
    }

    private void UpdateDriverConfiguration<T>(IBasicHardwareDeviceConfiguration config)
    {
        var item = _dbContext.Configurations.Where(x => x.Key == typeof(T).Name).FirstOrDefault();

        if (item == null)
        {
            item = new Configuration(typeof(T).Name, System.Text.Json.JsonSerializer.Serialize(config, config.GetType(), _jsonSerializerOptions));
            _dbContext.Configurations.Add(item);
        }
        else
        {
            item.Value = System.Text.Json.JsonSerializer.Serialize(config, config.GetType(), _jsonSerializerOptions);
        }
    }

    public void UpdateBillAcceptorConfigurationAsync(IBillAcceptorConfiguration config) => UpdateDriverConfiguration<IBillAcceptorConfiguration>(config);
    public void UpdateBillDispenserConfigurationAsync(IBillDispenserConfiguration config) => UpdateDriverConfiguration<IBillDispenserConfiguration>(config);
    public void UpdateCabinetConfigurationAsync(ICabinetConfiguration config) => UpdateDriverConfiguration<ICabinetConfiguration>(config);
    public void UpdateUserCardReaderConfigurationAsync(IUserCardReaderConfiguration config) => UpdateDriverConfiguration<IUserCardReaderConfiguration>(config);
    public void UpdateTITOPrinterConfigurationAsync(ITITOPrinterConfiguration config) => UpdateDriverConfiguration<ITITOPrinterConfiguration>(config);
    public void UpdateCoinAcceptorConfigurationAsync(ICoinAcceptorConfiguration config) => UpdateDriverConfiguration<ICoinAcceptorConfiguration>(config);
    public void UpdateParcelLockerConfiguration(IParcelLockerConfiguration config) => UpdateDriverConfiguration<IParcelLockerConfiguration>(config);
    public void UpdateUpsConfigurationAsync(UpsConfiguration config) => UpdateConfiguration(config);
    public void UpdateMainConfigurationAsync(MainConfiguration config) => UpdateConfiguration(config);
    public void UpdateServerConfigurationAsync(ServerConfiguration config) => UpdateConfiguration(config);
    public void UpdateNetworkConfigurationAsync(NetworkConfiguration config) => UpdateConfiguration(config);
    public void UpdateRegionalConfigurationAsync(RegionalConfiguration config) => UpdateConfiguration(config);
    public void UpdateOnlineIntegrationsConfigurationAsync(OnlineIntegrationsConfiguration config) => UpdateConfiguration(config);
    public void UpdatePayoutRulesConfigurationAsync(PayoutRulesConfiguration config) => UpdateConfiguration(config);
    public void UpdateSoundConfigurationAsync(SoundConfiguration config) => UpdateConfiguration(config);
    public void UpdateSoundEventsConfigurationAsync(SoundEventsConfiguration config) => UpdateConfiguration(config);
    public void UpdateVideoConfigurationAsync(VideoConfiguration config) => UpdateConfiguration(config);
    public void UpdateFlashConfigurationAsync(FlashConfiguration config) => UpdateConfiguration(config);
    public void UpdateUserWidgetsConfiguration(UserWidgetsConfiguration config) => UpdateConfiguration(config);
    public void UpdateAvailableUserWidgetsConfiguration(AvailableUserWidgetsConfiguration config) => UpdateConfiguration(config);
    public void UpdatePostalServicesConfiguration(PostalServicesConfiguration config) => UpdateConfiguration(config);
    public void UpdateAvailablePostalServicesConfiguration(AvailablePostalServicesConfiguration config) => UpdateConfiguration(config);

    public void UpdateSupportedPaymentOptionsConfiguration(SupportedPaymentOptionsConfiguration config) => UpdateConfiguration(config);
    public async Task<UpsConfiguration> GetCurrentUpsConfigurationAsync()
    {
        return await GetConfiguration<UpsConfiguration>();
    }

    public async Task<MainConfiguration> GetCurrentMainConfigurationAsync()
    {
        return await GetConfiguration<MainConfiguration>();
    }

    public async Task<ServerConfiguration> GetCurrentServerConfigurationAsync()
    {
        return await GetConfiguration<ServerConfiguration>();
    }

    public async Task<NetworkConfiguration> GetCurrentNetworkConfigurationAsync()
    {
        return await GetConfiguration<NetworkConfiguration>();
    }

    public async Task<RegionalConfiguration> GetCurrentRegionalConfigurationAsync()
    {
        return await GetConfiguration<RegionalConfiguration>();
    }

    public async Task<OnlineIntegrationsConfiguration> GetCurrentOnlineIntegrationsConfigurationAsync()
    {
        return await GetConfiguration<OnlineIntegrationsConfiguration>();
    }

    public async Task<PayoutRulesConfiguration> GetPayoutRulesConfigurationAsync()
    {
        return await GetConfiguration<PayoutRulesConfiguration>();
    }

    public async Task<SoundConfiguration> GetSoundConfigurationAsync()
    {
        return await GetConfiguration<SoundConfiguration>();
    }

    public async Task<SoundEventsConfiguration> GetSoundEventsConfigurationAsync()
    {
        return await GetConfiguration<SoundEventsConfiguration>();
    }

    public async Task<VideoConfiguration> GetVideoConfigurationAsync()
    {
        return await GetConfiguration<VideoConfiguration>();
    }

    public async Task<FlashConfiguration> GetFlashConfigurationAsync()
    {
        return await GetConfiguration<FlashConfiguration>();
    }

    public async Task<UserWidgetsConfiguration> GetUserWidgetsConfigurationAsync()
    {
        return await GetConfiguration<UserWidgetsConfiguration>();
    }

    public async Task<AvailableUserWidgetsConfiguration> GetAvailableUserWidgetsConfigurationAsync()
    {
        return await GetConfiguration<AvailableUserWidgetsConfiguration>();
    }

    public async Task<IBillDispenserConfiguration?> GetBillDispenserConfigurationAsync()
    {
        return (IBillDispenserConfiguration?)(await GetDriverConfiguration<IBillDispenserConfiguration>(DeviceType.BillDispenser));
    }

    public async Task<ICabinetConfiguration?> GetCabinetConfigurationAsync()
    {
        return (ICabinetConfiguration?)(await GetDriverConfiguration<ICabinetConfiguration>(DeviceType.Cabinet));
    }

    public async Task<IUserCardReaderConfiguration?> GetUserCardReaderConfigurationAsync()
    {
        return (IUserCardReaderConfiguration?)(await GetDriverConfiguration<IUserCardReaderConfiguration>(DeviceType.UserCardReader));
    }

    public async Task<ITITOPrinterConfiguration?> GetTITOPrinterConfigurationAsync()
    {
        return (ITITOPrinterConfiguration?)(await GetDriverConfiguration<ITITOPrinterConfiguration>(DeviceType.TITOPrinter));
    }

    public async Task<IBillAcceptorConfiguration?> GetBillAcceptorConfigurationAsync()
    {
        return (IBillAcceptorConfiguration?)(await GetDriverConfiguration<IBillAcceptorConfiguration>(DeviceType.BillAcceptor));
    }

    public async Task<ICoinAcceptorConfiguration?> GetCoinAcceptorConfigurationAsync()
    {
        return (ICoinAcceptorConfiguration?)(await GetDriverConfiguration<ICoinAcceptorConfiguration>(DeviceType.CoinAcceptor));
    }

    public async Task<IParcelLockerConfiguration> GetParcelLockerConfigurationAsync()
    {
        return (IParcelLockerConfiguration?)(await GetDriverConfiguration<IParcelLockerConfiguration>(DeviceType.ParcelLocker));
    }

    public async Task<PostalServicesConfiguration> GetPostalServicesConfigurationAsync()
    {
        return await GetConfiguration<PostalServicesConfiguration>();
    }

    public async Task<AvailablePostalServicesConfiguration> GetAvailablePostalServicesConfigurationAsync()
    {
        return await GetConfiguration<AvailablePostalServicesConfiguration>();
    }

    public async Task<SupportedPaymentOptionsConfiguration> GetSupportedPaymentOptionsConfigurationAsync()
    {
        return await GetConfiguration<SupportedPaymentOptionsConfiguration>();
    }

}
