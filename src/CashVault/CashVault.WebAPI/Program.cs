using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CashVault.Application;
using CashVault.Application.Interfaces;
using CashVault.Infrastructure;
using CashVault.WebAPI;
using CashVault.WebAPI.Helpers;
using CashVault.WebAPI.Hubs;
using CashVault.WebAPI.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

Microsoft.Extensions.Logging.ILogger? logger = null;

const string mutexName = @"Global\CashVault_Backbone";

var mutex = new Mutex(true, mutexName, out var createdNew);

if (!createdNew)
{
    Console.WriteLine(mutexName + " is already running! Exiting the application.");
    return;
}

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Check if encrypted configuration should be used
    var (aesKey, aesIv) = ConfigurationDecryption.LoadEncryptionKeys();
    string encryptedConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.enc.json");
    
    if (!string.IsNullOrEmpty(aesKey) && !string.IsNullOrEmpty(aesIv) && File.Exists(encryptedConfigPath))
    {
        // Loading encrypted configuration
        try
        {
            string decryptedJson = ConfigurationDecryption.DecryptAppSettings(encryptedConfigPath, aesKey, aesIv);
            using var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(decryptedJson));
            
            builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonStream(jsonStream)
                .AddEnvironmentVariables()
                .Build();
            
        }
        catch (Exception ex)
        {
           // Fallback to standard configuration on error 
            builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
    else
    {
        // Standard configuration loading
        builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    //((IConfigurationBuilder)builder.Configuration)
    //    .Add(new DatabaseConfigurationSource(builder.Configuration.GetConnectionString("CashVaultDatabase")!));

    builder.Host.UseSerilog((context, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);

        //string? serverConfigString = context.Configuration["ServerConfiguration"];

        //if (!string.IsNullOrEmpty(serverConfigString))
        //{
        //    ServerConfiguration? serverConfig;

        //    try
        //    {
        //        serverConfig = JsonSerializer.Deserialize<ServerConfiguration>(serverConfigString);

        //        if (serverConfig != null && serverConfig?.IsEnabled == true)
        //        {
        //            loggerConfiguration.WriteTo.Http($"{serverConfig.ServerUrl}/logs", queueLimitBytes: null, batchFormatter: new SerilogBatchFormatter(serverConfig.DeviceId));
        //            loggerConfiguration.Enrich.WithProperty("DeviceId", serverConfig.DeviceId ?? "001");

        //            if (serverConfig.MinimalLogLevel == (int)LogLevel.Information)
        //            {
        //                loggerConfiguration.MinimumLevel.Information();
        //            }
        //            else if (serverConfig.MinimalLogLevel == (int)LogLevel.Warning)
        //            {
        //                loggerConfiguration.MinimumLevel.Warning();
        //            }
        //            else if (serverConfig.MinimalLogLevel == (int)LogLevel.Error)
        //            {
        //                loggerConfiguration.MinimumLevel.Error();
        //            }
        //            else if (serverConfig.MinimalLogLevel == (int)LogLevel.Critical)
        //            {
        //                loggerConfiguration.MinimumLevel.Fatal();
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        logger?.LogError("Failed to deserialize ServerConfiguration");
        //    }
        //}
    });

    builder.Services.AddLogging(loggingBuilder =>
    {
        loggingBuilder.AddSerilog(dispose: true);
    });

    // Add services to the container.
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices()
        .AddServerHeartBeatService(builder.Configuration)
        .AddLocalDevEnvOptions(builder.Configuration);
    builder.Services.AddWebAPIServices(builder.Configuration);

    builder.Services.AddHttpLogging(logging =>
    {
        logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
    });

    var app = builder.Build();
    logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Starting CashVault core...");

    CashVault.Infrastructure.ConfigureServices.CheckInfrastructurePrerequisites(app.Services, logger);

    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;

            var response = new
            {
                status = report.Status.ToString(),
                errors = report.Entries.Select(e => new
                {
                    key = e.Key,
                    value = Enum.GetName(typeof(HealthStatus), e.Value.Status)
                })
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    });

    app.UseCors("AllowOnlyLocalhost");
    app.UseHttpLogging();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMiddleware<SessionMiddleware>();
    app.MapControllers();
    app.MapHub<MoneyServicesHub>("/moneyserviceshub");
    app.MapHub<DeviceEventsHub>("/deviceeventshub");
    app.MapHub<HeartbeatHub>("/heartbeathub");


    var startupService = app.Services.GetRequiredService<IStartupService>();
    app.Lifetime.ApplicationStarted.Register(startupService.OnApplicationStarted);
    app.Lifetime.ApplicationStopping.Register(startupService.OnApplicationStopping);

    app.Run();

}
catch (Exception ex)
{
    logger?.LogCritical(ex, "Application terminated unexpectedly.");
}