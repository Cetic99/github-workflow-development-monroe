using CashVault.Application.Interfaces;
using CashVault.Application.Interfaces.Persistence;
using CashVault.DeviceDriver.Common;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common;
using CashVault.Infrastructure.CMSService;
using CashVault.Infrastructure.Configuration;
using CashVault.Infrastructure.DeviceHardware;
using CashVault.Infrastructure.PersistentStorage;
using CashVault.Infrastructure.PersistentStorage.Interceptors;
using CashVault.Infrastructure.PersistentStorage.Repositories;
using CashVault.Infrastructure.PostalServiceProviders;
using CashVault.Infrastructure.Server;
using CashVault.Infrastructure.TicketProviders;
using CashVault.Infrastructure.TicketProviders.Betbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CashVault.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IAppInfoService, AppInfoService>();
        services.AddSingleton<ITerminal, Terminal>();
        services.AddSingleton<IDeviceDriverFactory, DeviceDriverFactory>();
        services.AddHttpClient();

        services.AddScoped<EntitySaveChangesInterceptor>();
        services.AddDbContext<CashVaultContext>();
        services.AddScoped<ICMSService, CMSService.CMSService>();
        services.AddHostedService<CMSConnectivityChecker>();
        services.AddTicketProviders();
        services.AddPostalServiceProviders();

        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IOperatorRepository, OperatorRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ITerminalRepository, TerminalRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IMoneyStatusRepository, MoneyStatusRepository>();
        services.AddScoped<IEventLogRepository, EventLogRepository>();
        services.AddScoped<IParcelLockerRepository, ParcelLockerRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static void CheckInfrastructurePrerequisites(IServiceProvider services, ILogger logger)
    {
        try
        {
            CheckDatabaseIsAvailable(services, logger);
        }
        catch (Exception ex)
        {
            logger.LogError("Infrastructure prerequisites check failed.");
            throw;
        }

    }

    private static void CheckDatabaseIsAvailable(IServiceProvider services, ILogger logger)
    {
        try
        {
            using (var scope = services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CashVaultContext>();

                dbContext.Database.OpenConnection();
                dbContext.Database.CloseConnection();
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Database is not available.");
            throw;
        }
    }

    public static IServiceCollection AddServerHeartBeatService(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration
                .GetSection("Heartbeat")
                .Get<HeartbeatSettings>()
                ?? throw new ArgumentNullException(nameof(HeartbeatSettings));

        services.AddSingleton(settings);

        if (string.IsNullOrEmpty(settings.PrivateKeyPath))
        {
            throw new InvalidOperationException("Private key path is missing in configuration.");
        }

        var privateKeyPem = File.ReadAllText(settings.PrivateKeyPath);

        if (string.IsNullOrEmpty(privateKeyPem))
        {
            throw new InvalidOperationException("Private key is missing in configuration.");
        }

        services.AddSingleton(new SignatureProvider(privateKeyPem,
                                                    Enumeration.GetByCode<HashAlgorithmType>(settings.HashAlgorithm),
                                                    Enumeration.GetByCode<SignatureAlgorithmType>(settings.SignatureAlgorithm)));
        services.AddHostedService<ServerHeartbeatService>();
        services.AddHostedService<EventsSenderService>();

        return services;
    }

    public static IServiceCollection AddLocalDevEnvOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LocalDevEnvOptions>(configuration.GetSection("LocalDevEnvOptions"));

        return services;
    }

    public static IServiceCollection AddTicketProviders(this IServiceCollection services)
    {
        services.AddScoped<ITicketProviderFactory, TicketProviderFactory>();
        services.AddScoped<ITicketProvider, BetboxTicketProvider>();

        return services;
    }

    public static IServiceCollection AddPostalServiceProviders(this IServiceCollection services)
    {
        services.AddScoped<IPostalServiceProvider, PostalServiceProvider>();

        return services;
    }
}
