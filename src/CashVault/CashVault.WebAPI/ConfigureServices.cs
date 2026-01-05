using System.Text;
using CashVault.Application.Common.Helpers;
using CashVault.Application.Interfaces;
using CashVault.Infrastructure.Configuration;
using CashVault.WebAPI.Common;
using CashVault.WebAPI.FIlters;
using CashVault.WebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

namespace CashVault.WebAPI;

public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add services here
        services.AddHttpContextAccessor();

        services.AddSingleton<INotificationService, NotificationService>();
        services.AddHostedService<OperatingModeService>();
        services.AddHostedService<HeartbeatService>();

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new StringToIntJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new StringToBoolJsonConverter());
            });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHealthChecks();

        AddSignalR(services);
        AddCorsOptions(services);
        AddAuthOptions(services, configuration);

        return services;
    }

    private static IServiceCollection AddSignalR(IServiceCollection services)
    {
        services.AddSignalR(options =>
        {
            // Global filters will run first
            options.AddFilter<AuthenticationHubFilter>();
        });

        return services;
    }

    private static IServiceCollection AddAuthOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JWTSettings>(configuration.GetSection("JWT"));
        var jwtConfig = new JWTSettings();
        configuration.GetSection("JWT").Bind(jwtConfig);

        services.AddSingleton(jwtConfig);
        services.AddSingleton<JwtTokenHelper>();

        services.AddAuthentication(cfg =>
        {
            cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = false;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.FromSeconds(15)
            };
        });

        services.AddAuthorization();
        services.AddScoped<ISessionService, SessionService>();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();

        return services;
    }

    private static IServiceCollection AddCorsOptions(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowOnlyLocalhost",
                               builder =>
                               {
                                   builder
                                       //.SetIsOriginAllowed(origin => true)
                                       .WithOrigins("http://localhost:5173", "null")
                                       .AllowAnyHeader()
                                       .AllowAnyMethod()
                                       .AllowCredentials();
                               });
        });

        return services;
    }
}
