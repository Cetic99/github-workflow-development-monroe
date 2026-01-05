using System.Text;
using System.Text.Json;
using CashVault.Application.Interfaces;
using CashVault.Infrastructure.CMSService.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CashVault.Infrastructure.CMSService
{
    internal class CMSConnectivityChecker : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CMSConnectivityChecker> _logger;
        private readonly INotificationService _notificationService;
        private readonly IServiceProvider _serviceProvider;

        private static TimeSpan _interval = TimeSpan.FromSeconds(10);
        private static TimeSpan _timeout = TimeSpan.FromSeconds(5);
        private static readonly JsonSerializerOptions _serializeOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public CMSConnectivityChecker(
            IHttpClientFactory httpClientFactory,
            ILogger<CMSConnectivityChecker> logger,
            INotificationService notificationService,
            IServiceProvider serviceProvider)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _notificationService = notificationService;
            _serviceProvider = serviceProvider;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

                    if (unitOfWork == null)
                    {
                        _logger.LogError("unitOfWork not available");
                        continue;
                    }

                    var onlineInterations = await unitOfWork.TerminalRepository.GetCurrentOnlineIntegrationsConfigurationAsync();

                    if (onlineInterations == null || !onlineInterations.CasinoManagementSystem || string.IsNullOrEmpty(onlineInterations.Url))
                    {
                        _logger.LogInformation("CMS is not configured. Skipping connectivity check.");
                        await _notificationService.CMSConnectivityStatus(new Domain.Common.CMSConnectivityStatusModel(false, false));
                        await Task.Delay(_interval, stoppingToken);
                        continue;
                    }
                    using var client = _httpClientFactory.CreateClient();
                    client.Timeout = _timeout;

                    var requestModel = new HealthCheckRequestDto(DateTime.UtcNow, onlineInterations.DeviceId, onlineInterations.SecretKey);
                    var json = JsonSerializer.Serialize(requestModel, _serializeOptions);

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(onlineInterations.Url, content);
                    bool isConnected = response.IsSuccessStatusCode;
                    _logger.LogInformation($"CMS connectivity check: {(isConnected ? "Success" : "Failure")}");

                    await _notificationService.CMSConnectivityStatus(new Domain.Common.CMSConnectivityStatusModel(isConnected, onlineInterations.CasinoManagementSystem));

                    await Task.Delay(_interval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during CMS connectivity check.");
                    await _notificationService.CMSConnectivityStatus(new Domain.Common.CMSConnectivityStatusModel(false, false));
                    await Task.Delay(_interval, stoppingToken);
                }
            }
        }
    }
}
