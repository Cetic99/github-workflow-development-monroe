using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common;
using CashVault.Domain.Common.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CashVault.Infrastructure.Server
{
    internal class EventsSenderService : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<EventsSenderService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITerminal _terminal;
        private readonly SignatureProvider _requestSigner;
        private readonly SignatureAlgorithmType _signatureAlgorithm = SignatureAlgorithmType.RSA;
        private readonly HashAlgorithmType _hashAlgorithm = HashAlgorithmType.SHA512;
        private readonly int _sendIntervalDefault = 3;

        public EventsSenderService(IHttpClientFactory httpClientFactory,
                                   ILogger<EventsSenderService> logger,
                                   IServiceProvider serviceProvider,
                                   ITerminal terminal,
                                   SignatureProvider requestSigner)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _terminal = terminal;
            _requestSigner = requestSigner;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ServerConfiguration? serverConfig = _terminal.ServerConfiguration;

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

                    var events = await unitOfWork.EventLogRepository.GetUnsentEvents();

                    if (events.Count == 0)
                    {
                        _logger.LogTrace("No unsent events found. Skiping sending events.");
                        await Task.Delay(TimeSpan.FromSeconds(_sendIntervalDefault), stoppingToken);
                        continue;
                    }

                    if (!IsServerConfigured(serverConfig))
                    {
                        _logger.LogTrace("Server is not configured. Skiping sending events.");
                        await Task.Delay(TimeSpan.FromSeconds(_sendIntervalDefault), stoppingToken);
                        continue;
                    }

                    bool isSentToRemoteServer = await SendEventsToRemoteServer(events, serverConfig, stoppingToken);

                    if (isSentToRemoteServer)
                    {
                        events.ForEach(e => e.MarkAsSentToRemote());
                        unitOfWork.EventLogRepository.UpdateEvents(events);
                        await unitOfWork.SaveChangesAsync();
                    }

                    await Task.Delay(TimeSpan.FromSeconds(serverConfig.SendInterval), stoppingToken);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errr sending data to remote server.");
                    await Task.Delay(TimeSpan.FromSeconds(serverConfig?.SendInterval ?? _sendIntervalDefault), stoppingToken);
                }
            }
        }

        private async Task<bool> SendEventsToRemoteServer(List<BaseEvent> events, ServerConfiguration serverConfig, CancellationToken stoppingToken)
        {
            var payload = new EventRequestToRemoteServer(DateTime.UtcNow.ToString("o"), serverConfig.DeviceId, events);

            var jsonPayload = SignatureProvider.CanonicalizeJson(payload);
            var signedRequest = _requestSigner.SignRequest(jsonPayload, _hashAlgorithm, _signatureAlgorithm);
            var request = SignatureProvider.ConstructRequest($"{serverConfig.ServerUrl}/events", jsonPayload, signedRequest, _hashAlgorithm, _signatureAlgorithm);
            using var httpClient = _httpClientFactory.CreateClient();

            _logger.LogTrace("Sending data to remote server.");

            var response = await httpClient.SendAsync(request, stoppingToken);
            var responseString = await response.Content.ReadAsStringAsync(stoppingToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogTrace($"Server communication successful, response: {responseString}.");
                return true;
            }
            else
            {
                _logger.LogError($"Server communication failed, status code: {response.StatusCode}, details: {responseString}");
            }

            return false;
        }

        private bool IsServerConfigured(ServerConfiguration? serverConfig)
        {
            return serverConfig != null && serverConfig.IsEnabled && !string.IsNullOrEmpty(serverConfig.ServerUrl);
        }
    }
}
