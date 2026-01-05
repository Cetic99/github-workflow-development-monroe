using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Events;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.MoneyStatusAggregate.Events;
using CashVault.Domain.Aggregates.TicketAggregate.Events;
using CashVault.Domain.Aggregates.TransactionAggregate.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CashVault.Application.Services;

public class StartupService : IStartupService
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITerminal terminal;

    public StartupService(IServiceProvider serviceProvider, ITerminal terminal)
    {
        this.serviceProvider = serviceProvider;
        this.terminal = terminal;
    }

    public void AddEventDispatching(ITerminal terminal)
    {
        if (terminal == null)
        {
            throw new ArgumentNullException(nameof(terminal));
        }

        if (terminal.BillAcceptor != null)
        {
            AddBillAcceptorEventDispatching(terminal.BillAcceptor);
        }

        if (terminal.CoinAcceptor != null)
        {
            AddCoinAcceptorEventDispatching(terminal.CoinAcceptor);
        }

        if (terminal.BillDispenser != null)
        {
            AddBillDispenserEventDispatching(terminal.BillDispenser);
        }

        if (terminal.UserCardReader != null)
        {
            AddCardReaderEventDispatching(terminal.UserCardReader);
        }

        if (terminal.TITOPrinter != null)
        {
            AddTITOPrinterEventDispatching(terminal.TITOPrinter);
        }

        if (terminal.Cabinet != null)
        {
            AddCabinetEventDispatching(terminal.Cabinet);
        }
    }

    public async void OnApplicationStarted()
    {

        using (var scope = serviceProvider.CreateScope())
        {
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            var regionalService = scope.ServiceProvider.GetService<IRegionalService>();
            var terminalRepository = unitOfWork.TerminalRepository;
            var driverFactory = scope.ServiceProvider.GetService<IDeviceDriverFactory>();

            await regionalService.LoadMessages();

            // Get configurations from database
            var mainConfig = await terminalRepository.GetCurrentMainConfigurationAsync();

            if (mainConfig == null)
            {
                mainConfig = new MainConfiguration();
                terminalRepository.UpdateMainConfigurationAsync(mainConfig);
            }

            driverFactory.SetMainConfiguration(mainConfig);

            if (mainConfig.IsTITOPrinterMainConfigUpdated())
            {
                var titoPrinterConfig = await terminalRepository.GetTITOPrinterConfigurationAsync();
                if (titoPrinterConfig == null)
                {
                    titoPrinterConfig = (ITITOPrinterConfiguration)driverFactory.CreateConfiguration(DeviceType.TITOPrinter, null);
                }
                terminalRepository.UpdateTITOPrinterConfigurationAsync(titoPrinterConfig);
                terminal.SetTITOPrinterConfiguration(titoPrinterConfig);
            }
            var billDispenserConfig = await terminalRepository.GetBillDispenserConfigurationAsync();
            if (mainConfig.IsBillDispenserMainConfigUpdated())
            {
                if (billDispenserConfig == null)
                {
                    billDispenserConfig = (IBillDispenserConfiguration)driverFactory.CreateConfiguration(DeviceType.BillDispenser, null);
                }
                terminalRepository.UpdateBillDispenserConfigurationAsync(billDispenserConfig);
            }
            if (billDispenserConfig != null)
            {
                var billStatus = await unitOfWork.MoneyStatusRepository.GetDispenserBillCountStatusAsync();
                billStatus.UpdateBillCassetteFromDispenserConfig(billDispenserConfig.GetBillCassettes());
                unitOfWork.MoneyStatusRepository.UpdateDispenserBillCountStatus(billStatus);
                terminal.SetBillDispenserConfiguration(billDispenserConfig);

            }
            if (mainConfig.IsBillAcceptorMainConfigUpdated())
            {
                var billAcceptorConfig = await terminalRepository.GetBillAcceptorConfigurationAsync();
                if (billAcceptorConfig == null)
                {
                    billAcceptorConfig = (IBillAcceptorConfiguration)driverFactory.CreateConfiguration(DeviceType.BillAcceptor, null);
                }
                terminalRepository.UpdateBillAcceptorConfigurationAsync(billAcceptorConfig);
                terminal.SetBillAcceptorConfiguration(billAcceptorConfig);
            }
            if (mainConfig.IsUserCardReaderMainConfigUpdated())
            {
                var userCardReaderConfig = await terminalRepository.GetUserCardReaderConfigurationAsync();
                if (userCardReaderConfig == null)
                {
                    userCardReaderConfig = (IUserCardReaderConfiguration)driverFactory.CreateConfiguration(DeviceType.UserCardReader, null);
                }
                terminalRepository.UpdateUserCardReaderConfigurationAsync(userCardReaderConfig);
                terminal.SetUserCardReaderConfiguration(userCardReaderConfig);
            }
            if (mainConfig.IsCabinetMainConfigUpdated())
            {
                var cabinetConfig = await terminalRepository.GetCabinetConfigurationAsync();
                if (cabinetConfig == null)
                {
                    cabinetConfig = (ICabinetConfiguration)driverFactory.CreateConfiguration(DeviceType.Cabinet, null);
                }
                terminalRepository.UpdateCabinetConfigurationAsync(cabinetConfig);
                terminal.SetCabinetConfiguration(cabinetConfig);
            }
            if (mainConfig.IsCoinAcceptorMainConfigUpdated())
            {
                var coinAcceptorConfig = await terminalRepository.GetCoinAcceptorConfigurationAsync();
                if (coinAcceptorConfig == null)
                {
                    coinAcceptorConfig = (ICoinAcceptorConfiguration)driverFactory.CreateConfiguration(DeviceType.CoinAcceptor, null);
                }
                terminalRepository.UpdateCoinAcceptorConfigurationAsync(coinAcceptorConfig);
                terminal.SetCoinAcceptorConfiguration(coinAcceptorConfig);
            }

            if (mainConfig.IsParcelLockerMainConfigUpdated())
            {
                var parcelLockerConfig = await terminalRepository.GetParcelLockerConfigurationAsync();

                if (parcelLockerConfig == null)
                {
                    parcelLockerConfig = (IParcelLockerConfiguration)driverFactory.CreateConfiguration(DeviceType.ParcelLocker, null);
                    terminalRepository.UpdateParcelLockerConfiguration(parcelLockerConfig);
                }
                terminal.SetParcelLockerConfiguration(parcelLockerConfig);
            }

            var regionalConfig = await terminalRepository.GetCurrentRegionalConfigurationAsync();
            var networkConfig = await terminalRepository.GetCurrentNetworkConfigurationAsync();
            var upsConfig = await terminalRepository.GetCurrentUpsConfigurationAsync();
            var onlineIntegrationsConfig = await terminalRepository.GetCurrentOnlineIntegrationsConfigurationAsync();
            var serverConfig = await terminalRepository.GetCurrentServerConfigurationAsync();

            var payoutRulesConfig = await terminalRepository.GetPayoutRulesConfigurationAsync();
            var soundConfig = await terminalRepository.GetSoundConfigurationAsync();
            var soundEventsConfig = await terminalRepository.GetSoundEventsConfigurationAsync();
            var videoConfig = await terminalRepository.GetVideoConfigurationAsync();
            var flashConfig = await terminalRepository.GetFlashConfigurationAsync();

            var userWidgetsConfig = await terminalRepository.GetUserWidgetsConfigurationAsync();
            var availableUserWidgetsConfig = await terminalRepository.GetAvailableUserWidgetsConfigurationAsync();

            var postalServicesConfig = await terminalRepository.GetPostalServicesConfigurationAsync();
            var availablePostalServicesConfig = await terminalRepository.GetAvailablePostalServicesConfigurationAsync();
            var supportedPaymentOptionsConfig = await terminalRepository.GetSupportedPaymentOptionsConfigurationAsync();

            // Init default configurations if null
            if (serverConfig == null)
            {
                serverConfig = new ServerConfiguration();
                terminalRepository.UpdateServerConfigurationAsync(serverConfig);
            }

            if (upsConfig == null)
            {
                upsConfig = new UpsConfiguration();
                terminalRepository.UpdateUpsConfigurationAsync(upsConfig);
            }

            if (regionalConfig == null)
            {
                regionalConfig = new RegionalConfiguration();
                terminalRepository.UpdateRegionalConfigurationAsync(regionalConfig);
            }

            if (networkConfig == null)
            {
                networkConfig = new NetworkConfiguration();
                terminalRepository.UpdateNetworkConfigurationAsync(networkConfig);
            }

            if (onlineIntegrationsConfig == null)
            {
                onlineIntegrationsConfig = new OnlineIntegrationsConfiguration();
                terminalRepository.UpdateOnlineIntegrationsConfigurationAsync(onlineIntegrationsConfig);
            }

            if (payoutRulesConfig == null)
            {
                payoutRulesConfig = new PayoutRulesConfiguration();
                terminalRepository.UpdatePayoutRulesConfigurationAsync(payoutRulesConfig);
            }

            if (soundConfig == null)
            {
                soundConfig = new SoundConfiguration();
                terminalRepository.UpdateSoundConfigurationAsync(soundConfig);
            }

            if (soundEventsConfig == null)
            {
                soundEventsConfig = new SoundEventsConfiguration();
                terminalRepository.UpdateSoundEventsConfigurationAsync(soundEventsConfig);
            }

            if (videoConfig == null)
            {
                videoConfig = new VideoConfiguration();
                terminalRepository.UpdateVideoConfigurationAsync(videoConfig);
            }

            if (flashConfig == null)
            {
                flashConfig = new FlashConfiguration();
                terminalRepository.UpdateFlashConfigurationAsync(flashConfig);
            }

            if (availableUserWidgetsConfig == null)
            {
                availableUserWidgetsConfig = new AvailableUserWidgetsConfiguration();
                availableUserWidgetsConfig.Initialize();

                terminalRepository.UpdateAvailableUserWidgetsConfiguration(availableUserWidgetsConfig);
            }

            if (userWidgetsConfig == null)
            {
                userWidgetsConfig = new UserWidgetsConfiguration();
                userWidgetsConfig.Initialize();

                terminalRepository.UpdateUserWidgetsConfiguration(userWidgetsConfig);
            }

            if (availablePostalServicesConfig == null)
            {
                availablePostalServicesConfig = new AvailablePostalServicesConfiguration();
                availablePostalServicesConfig.Initialize();

                terminalRepository.UpdateAvailablePostalServicesConfiguration(availablePostalServicesConfig);
            }

            if (postalServicesConfig == null)
            {
                postalServicesConfig = new PostalServicesConfiguration();
                postalServicesConfig.Initialize();

                terminalRepository.UpdatePostalServicesConfiguration(postalServicesConfig);
            }

            if (supportedPaymentOptionsConfig == null)
            {
                supportedPaymentOptionsConfig = new SupportedPaymentOptionsConfiguration();
                supportedPaymentOptionsConfig.Initialize();

                terminalRepository.UpdateSupportedPaymentOptionsConfiguration(supportedPaymentOptionsConfig);
            }

            // Set the configurations on the Terminal
            terminal.SetServerConfiguration(serverConfig);
            terminal.SetNetworkConfiguration(networkConfig);
            terminal.SetOnlineIntegrationsConfiguration(onlineIntegrationsConfig);
            terminal.SetRegionalConfiguration(regionalConfig);
            terminal.SetUpsConfiguration(upsConfig);
            terminal.UpdateLocalTimeZone(regionalConfig.LocalTimeZone);
            terminal.UpdateAmountPrecision(regionalConfig.AmountPrecision);
            terminal.SetMainConfiguration(mainConfig);

            await unitOfWork.SaveChangesAsync();
        }

        // Start the terminal
        terminal.StartAsync().Wait();
        AddEventDispatching(terminal);
    }

    public void OnApplicationStopping()
    {
        terminal.StartAsync().Wait();
    }

    private void AddBillAcceptorEventDispatching(IBillAcceptor billAcceptor)
    {
        billAcceptor.BillAcceptingStarted += async (sender, message) =>
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var notification = new BillAcceptingStartedEvent();
                await mediator.Publish(notification);
            }
        };

        billAcceptor.BillAccepted += async (sender, amount) =>
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var notification = new BillAcceptedEvent(amount);
                await mediator.Publish(notification);
            }
        };

        billAcceptor.BillRejected += async (sender, message) =>
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var notification = new BillRejectedEvent(message);

                terminal.AddDomainEvent(notification);
                await unitOfWork.SaveChangesAsync();
            }
        };

        billAcceptor.TicketAccepted += async (sender, message) =>
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var notification = new TicketAcceptedEvent(message);

                terminal.AddDomainEvent(notification);
                await unitOfWork.SaveChangesAsync();
            }
        };

        billAcceptor.TicketRejected += async (sender, message) =>
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var notification = new TicketRejectedEvent(message);

                terminal.AddDomainEvent(notification);
                await unitOfWork.SaveChangesAsync();
            }
        };

        billAcceptor.StackBoxRemoved += async (sender, e) =>
        {
            // TODO: implement this
        };

        billAcceptor.StackBoxFull += async (sender, e) =>
        {
            // TODO: implement this
        };

        billAcceptor.JamInStacker += async (sender, e) =>
        {
            // TODO: implement this
        };

        billAcceptor.JamInAcceptor += async (sender, e) =>
        {
            // TODO: implement this
        };

        billAcceptor.ErrorOccured += async (sender, message) =>
        {
            using var scope = this.serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            terminal.AddDeviceError(new DeviceErrorOccuredEvent(DeviceType.BillAcceptor, message));

            await unitOfWork.SaveChangesAsync();
        };
        billAcceptor.WarningRaised += async (sender, message) =>
        {
            using var scope = this.serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            terminal.AddWarningRaised(new DeviceWarningRaisedEvent(DeviceType.BillAcceptor, message));
            await unitOfWork.SaveChangesAsync();
        };
    }

    private void AddCoinAcceptorEventDispatching(ICoinAcceptor coinAcceptor)
    {
        coinAcceptor.CoinAccepted += async (sender, amount) =>
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var notification = new CoinAcceptedEvent(amount);
                await mediator.Publish(notification);
            }
        };

        coinAcceptor.CoinRejected += async (sender, message) =>
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var notification = new CoinRejectedEvent(message);

                terminal.AddDomainEvent(notification);
                await unitOfWork.SaveChangesAsync();
            }
        };

        coinAcceptor.ErrorOccured += async (sender, message) =>
        {
            using var scope = this.serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            terminal.AddDeviceError(new DeviceErrorOccuredEvent(DeviceType.CoinAcceptor, message));

            await unitOfWork.SaveChangesAsync();
        };
        coinAcceptor.WarningRaised += async (sender, message) =>
        {
            using var scope = this.serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            terminal.AddWarningRaised(new DeviceWarningRaisedEvent(DeviceType.CoinAcceptor, message));
            await unitOfWork.SaveChangesAsync();
        };
    }

    private void AddBillDispenserEventDispatching(IBillDispenser billDispenser)
    {
        // TODO: add eents for bill dispenser
        billDispenser.ErrorOccured += async (sender, message) =>
        {
            using var scope = this.serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            terminal.AddDeviceError(new DeviceErrorOccuredEvent(DeviceType.BillDispenser, message));

            await unitOfWork.SaveChangesAsync();
        };

        billDispenser.WarningRaised += async (sender, message) =>
        {
            using var scope = this.serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            terminal.AddWarningRaised(new DeviceWarningRaisedEvent(DeviceType.BillDispenser, message));
            await unitOfWork.SaveChangesAsync();
        };
    }

    private void AddCardReaderEventDispatching(IUserCardReader cardReader)
    {
        cardReader.CardAuthenticated += async (sender, args) =>
        {
            var (cardUID, cardUuid) = args;

            if (terminal.UserLoginEnabled)
            {
                using var scope = serviceProvider.CreateScope();

                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var notification = new CardAuthenticatedEvent(cardUuid, cardUID);

                terminal.AddDomainEvent(notification);
                await unitOfWork.SaveChangesAsync();
            }
        };

        cardReader.CardEnrolled += async (sender, cardIdRaw) =>
        {
            using var scope = serviceProvider.CreateScope();
            var cardUuid = Guid.TryParse(cardIdRaw, out var guid) ? guid : Guid.Empty;

            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var notification = new CardEnrolledEvent(cardUuid);

            terminal.AddDomainEvent(notification);
            await unitOfWork.SaveChangesAsync();
        };

        cardReader.CardAuthenticationFailed += async (sender, errorMessage) =>
        {
            using var scope = serviceProvider.CreateScope();

            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var notification = new CardAuthenticationFailedEvent(errorMessage);

            terminal.AddDomainEvent(notification);
            await unitOfWork.SaveChangesAsync();
        };

        cardReader.CardEnrollmentFailed += async (sender, errorMessage) =>
        {
            using var scope = serviceProvider.CreateScope();

            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var notification = new CardEnrollmentFailedEvent(errorMessage);

            terminal.AddDomainEvent(notification);
            await unitOfWork.SaveChangesAsync();
        };

        cardReader.ErrorOccured += async (sender, message) =>
        {
            using var scope = this.serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            terminal.AddDeviceError(new DeviceErrorOccuredEvent(DeviceType.UserCardReader, message));

            await unitOfWork.SaveChangesAsync();
        };

        cardReader.WarningRaised += async (sender, message) =>
        {
            using var scope = this.serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            terminal.AddWarningRaised(new DeviceWarningRaisedEvent(DeviceType.UserCardReader, message));
            await unitOfWork.SaveChangesAsync();
        };
    }

    private void AddTITOPrinterEventDispatching(ITITOPrinter titoPrinter)
    {
        titoPrinter.TicketPrintingStarted += async (sender, e) =>
        {
            // TODO: implement this
        };

        titoPrinter.TicketPrintingFailed += async (sender, e) =>
        {
            // TODO: implement this
        };

        titoPrinter.TicketPrintingCompleted += async (sender, e) =>
        {
            // TODO: implement this
        };
        titoPrinter.ErrorOccured += async (sender, message) =>
        {
            using var scope = this.serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            terminal.AddDeviceError(new DeviceErrorOccuredEvent(DeviceType.TITOPrinter, message));

            await unitOfWork.SaveChangesAsync();
        };

        titoPrinter.WarningRaised += async (sender, message) =>
        {
            using var scope = this.serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            terminal.AddWarningRaised(new DeviceWarningRaisedEvent(DeviceType.TITOPrinter, message));
            await unitOfWork.SaveChangesAsync();
        };
    }

    private void AddCabinetEventDispatching(ICabinet cabinet)
    {
        cabinet.VibrationDetected += async (sender, e) =>
        {
            // TODO: implement this
        };

        cabinet.DoorOpened += async (sender, doorNumber) =>
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                terminal.AddDomainEvent(new CabinetDoorOpenedEvent(doorNumber));
                await unitOfWork.SaveChangesAsync();
            }
        };

        cabinet.DoorClosed += async (sender, doorNumber) =>
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                terminal.AddDomainEvent(new CabinetDoorClosedEvent(doorNumber));
                await unitOfWork.SaveChangesAsync();
            }
        };
        cabinet.ErrorOccured += async (sender, message) =>
        {
            using var scope = this.serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            terminal.AddDeviceError(new DeviceErrorOccuredEvent(DeviceType.Cabinet, message));
            await unitOfWork.SaveChangesAsync();
        };
        cabinet.WarningRaised += async (sender, message) =>
        {
            using var scope = this.serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            terminal.AddWarningRaised(new DeviceWarningRaisedEvent(DeviceType.Cabinet, message));
            await unitOfWork.SaveChangesAsync();
        };
    }
}
