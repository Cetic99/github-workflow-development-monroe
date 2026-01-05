using System.Reflection;
using CashVault.Application.Common.Behaviors;
using CashVault.Application.Interfaces;
using CashVault.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CashVault.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            cfg.AddOpenBehavior(typeof(UnhandledExceptionBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));

        });

        services.AddTransient<INotificationPublisher, EventSynchronizationBehavior>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandSynchronizationBehavior<,>));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient<IRegionalService, RegionalService>();
        services.AddTransient<ILocalizer, RegionalService>();
        services.AddSingleton<IStartupService, StartupService>();

        return services;
    }
}
