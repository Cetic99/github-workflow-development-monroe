using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

namespace CashVault.Application.Interfaces.Persistence;

public interface ITerminalRepository
{
    void UpdateUpsConfigurationAsync(UpsConfiguration config);
    void UpdateMainConfigurationAsync(MainConfiguration config);
    void UpdateServerConfigurationAsync(ServerConfiguration config);
    void UpdateNetworkConfigurationAsync(NetworkConfiguration config);
    void UpdateRegionalConfigurationAsync(RegionalConfiguration config);
    void UpdateOnlineIntegrationsConfigurationAsync(OnlineIntegrationsConfiguration config);
    void UpdateBillAcceptorConfigurationAsync(IBillAcceptorConfiguration config);
    void UpdateBillDispenserConfigurationAsync(IBillDispenserConfiguration config);
    void UpdateCabinetConfigurationAsync(ICabinetConfiguration config);
    void UpdateUserCardReaderConfigurationAsync(IUserCardReaderConfiguration config);
    void UpdateTITOPrinterConfigurationAsync(ITITOPrinterConfiguration config);
    void UpdateCoinAcceptorConfigurationAsync(ICoinAcceptorConfiguration config);
    void UpdateParcelLockerConfiguration(IParcelLockerConfiguration config);
    void UpdatePayoutRulesConfigurationAsync(PayoutRulesConfiguration config);
    void UpdateSoundConfigurationAsync(SoundConfiguration config);
    void UpdateSoundEventsConfigurationAsync(SoundEventsConfiguration config);
    void UpdateVideoConfigurationAsync(VideoConfiguration config);
    void UpdateFlashConfigurationAsync(FlashConfiguration config);
    void UpdateUserWidgetsConfiguration(UserWidgetsConfiguration config);
    void UpdateAvailableUserWidgetsConfiguration(AvailableUserWidgetsConfiguration config);
    void UpdatePostalServicesConfiguration(PostalServicesConfiguration config);
    void UpdateAvailablePostalServicesConfiguration(AvailablePostalServicesConfiguration config);
    void UpdateSupportedPaymentOptionsConfiguration(SupportedPaymentOptionsConfiguration config);

    Task<UpsConfiguration> GetCurrentUpsConfigurationAsync();
    Task<MainConfiguration> GetCurrentMainConfigurationAsync();
    Task<ServerConfiguration> GetCurrentServerConfigurationAsync();
    Task<NetworkConfiguration> GetCurrentNetworkConfigurationAsync();
    Task<RegionalConfiguration> GetCurrentRegionalConfigurationAsync();
    Task<OnlineIntegrationsConfiguration> GetCurrentOnlineIntegrationsConfigurationAsync();
    Task<PayoutRulesConfiguration> GetPayoutRulesConfigurationAsync();
    Task<SoundConfiguration> GetSoundConfigurationAsync();
    Task<SoundEventsConfiguration> GetSoundEventsConfigurationAsync();
    Task<VideoConfiguration> GetVideoConfigurationAsync();
    Task<FlashConfiguration> GetFlashConfigurationAsync();
    Task<UserWidgetsConfiguration> GetUserWidgetsConfigurationAsync();
    Task<AvailableUserWidgetsConfiguration> GetAvailableUserWidgetsConfigurationAsync();
    Task<IBillDispenserConfiguration?> GetBillDispenserConfigurationAsync();
    Task<ITITOPrinterConfiguration?> GetTITOPrinterConfigurationAsync();
    Task<IBillAcceptorConfiguration?> GetBillAcceptorConfigurationAsync();
    Task<IUserCardReaderConfiguration?> GetUserCardReaderConfigurationAsync();
    Task<ICabinetConfiguration?> GetCabinetConfigurationAsync();
    Task<ICoinAcceptorConfiguration?> GetCoinAcceptorConfigurationAsync();
    Task<IParcelLockerConfiguration?> GetParcelLockerConfigurationAsync();
    Task<PostalServicesConfiguration> GetPostalServicesConfigurationAsync();
    Task<AvailablePostalServicesConfiguration> GetAvailablePostalServicesConfigurationAsync();
    Task<SupportedPaymentOptionsConfiguration> GetSupportedPaymentOptionsConfigurationAsync();
}
