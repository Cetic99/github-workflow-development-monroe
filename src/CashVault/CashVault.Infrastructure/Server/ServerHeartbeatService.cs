using System.Reflection;
using System.Text.Json;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using CashVault.Domain.Common;
using CashVault.Infrastructure.Server.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CashVault.Infrastructure.Server;

internal sealed class ServerHeartbeatService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ServerHeartbeatService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITerminal _terminal;
    private readonly SignatureProvider _requestSigner;
    private readonly SignatureAlgorithmType _signatureAlgorithm = SignatureAlgorithmType.RSA;
    private readonly HashAlgorithmType _hashAlgorithm = HashAlgorithmType.SHA512;
    private const int _sendIntervalDefault = 10;
    private readonly IAppInfoService _appInfoService;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    private static readonly JsonSerializerOptions _ignoreCaseCaseInsensitivOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ServerHeartbeatService(IHttpClientFactory httpClientFactory,
                               ILogger<ServerHeartbeatService> logger,
                               IServiceProvider serviceProvider,
                               ITerminal terminal,
                               SignatureProvider requestSigner,
                               IAppInfoService appInfoService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _terminal = terminal;
        _requestSigner = requestSigner;
        _appInfoService = appInfoService;
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

                if (serverConfig == null || !serverConfig.IsEnabled)
                {
                    _logger.LogDebug("Server is not configured. Skipping server communication.");
                    await Task.Delay(TimeSpan.FromSeconds(_sendIntervalDefault), stoppingToken);
                    continue;
                }

                var payload = await ConstructPayload(unitOfWork, _terminal);

                var jsonPayload = SignatureProvider.CanonicalizeJson(payload);
                var signedRequest = _requestSigner.SignRequest(jsonPayload, _hashAlgorithm, _signatureAlgorithm);

                var request = SignatureProvider.ConstructRequest($"{serverConfig.ServerUrl}/heartbeat", jsonPayload, signedRequest, _hashAlgorithm, _signatureAlgorithm);


                using var httpClient = _httpClientFactory.CreateClient();

                _logger.LogDebug($"Sending data to remote server: {serverConfig.ServerUrl}/heartbeat");
                _logger.LogDebug($"Heartbeat payload: {jsonPayload}");

                var response = await httpClient.SendAsync(request, stoppingToken);
                var responseString = await response.Content.ReadAsStringAsync(stoppingToken);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogDebug($"Server communication successful, response: {responseString}.");
                    HandleServerResponse(responseString);
                }
                else
                {
                    _logger.LogDebug($"Server communication failed, status code: {response.StatusCode}, details: {responseString}");
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

    private async Task<HeartbeatPayloadModel> ConstructPayload(IUnitOfWork unitOfWork, ITerminal terminal)
    {
        var totalAmount = 0.0m;
        BillTicketAcceptorStackerStatus acceptorMoneyStatus = await unitOfWork.MoneyStatusRepository.GetBillTicketAcceptorBillCountStatusAsync();
        DispenserBillCountStatus dispenserMoneyStatus = await unitOfWork.MoneyStatusRepository.GetDispenserBillCountStatusAsync();

        totalAmount += acceptorMoneyStatus?.BillAmount ?? 0.0m;
        totalAmount += dispenserMoneyStatus?.Cassettes.Sum(x => x.CurrentBillCount * x.BillDenomination) ?? 0.0m;

        var payload = new HeartbeatPayloadModel()
        {
            SoftwareVersion = _appInfoService.Version,
            TerminalStatus = _terminal.TerminalStatus, //TODO: Implement this
            TerminalTypes = _terminal.TerminalTypes,
            CurrentUser = "admin", //TODO: Current user should be added to the terminal
            OperatingMode = _terminal.OperatingMode,
            Timestamp = DateTime.UtcNow.ToString("o"), // ISO 8601
            DeviceId = terminal.ServerConfiguration.DeviceId
        };

        var devices = _terminal?.GetDevicesAsync();

        payload.PeripheralsStatuses = [];

        if (devices?.Any() ?? false)
        {
            foreach (var device in devices)
            {
                var status = await device.GetCurrentStatus();

                payload.PeripheralsStatuses.Add(new DeviceStatus()
                {
                    Type = BaseHelper.GetDeviceTypeCode(device),
                    Name = device.Name,
                    Status = status,
                    IsEnabled = device.IsEnabled,
                    IsConnected = device.IsConnected,
                    IsActive = device.IsActive,
                    Warning = device.GetWarning(),
                    Error = device.GetError(),
                    AdditionalInformation = device.GetAdditionalDeviceInfo()
                });
            }
        }

        payload.MoneyStatus = new Dtos.MoneyStatus()
        {
            TotalAmount = totalAmount,
            Acceptor = new Dtos.TicketAcceptorStackerMoneyStatus()
            {
                BillCount = acceptorMoneyStatus.BillCount,
                TicketCount = acceptorMoneyStatus.TicketCount,
                BillAmount = acceptorMoneyStatus.BillAmount,
                TicketAmount = acceptorMoneyStatus.TicketAmount
            },
            Dispenser = new Dtos.DispenserMoneyStatus()
            {
                Cassettes = dispenserMoneyStatus.Cassettes.Select(x => new Dtos.CassetteStatus()
                {
                    CassetteNumber = x.CassetteNumber,
                    Currency = x.Currency,
                    BillDenomination = x.BillDenomination,
                    CurrentBillCount = x.CurrentBillCount,
                }).ToList()
            }
        };

        var completedServerAction = _terminal?.CompletedServerActions.FirstOrDefault();
        if (completedServerAction != null)
        {
            payload.ActionResponse = new ServerActionResponseDto()
            {
                Uuid = completedServerAction.Uuid,
                ExternalId = completedServerAction.ExternalId,
                IsSuccess = completedServerAction.IsSuccess,
                Message = completedServerAction.ResponseMessage
            };
            await _terminal.RemoveServerActionAsync(completedServerAction.Uuid);
        }
        else
        {
            payload.ActionResponse = null;
        }

        return payload;
    }

    private void HandleServerResponse(string responseString)
    {
        if (string.IsNullOrEmpty(responseString))
            return;

        ServerResponse? serverResponse = null;

        try
        {
            serverResponse = JsonSerializer.Deserialize<ServerResponse>(responseString, _jsonSerializerOptions);
        }
        catch (Exception ex)
        {
            _logger.LogTrace(ex, "Unable to deserialize server response to Server command.");
        }

        if (serverResponse == null) return;

        var method = _terminal.GetType()
                .GetMethods()
                .FirstOrDefault(m => m.GetCustomAttribute<CanBeCalledFromRemoteServer>() != null &&
                                        m.Name.Equals(serverResponse.CommandName));

        if (method != null)
        {
            TerminalServerAction? serverAction;
            try
            {
                var parameters = method.GetParameters()
                    .Select(p =>
                    {
                        var param = serverResponse.Params.FirstOrDefault(x => x.Name == p.Name);
                        if (param == null) return Type.Missing;

                        if (param.Value is JsonElement je)
                        {
                            return je.Deserialize(p.ParameterType, _ignoreCaseCaseInsensitivOptions);
                        }

                        return Convert.ChangeType(param.Value, p.ParameterType);
                    })
                .ToArray();

                serverAction = new TerminalServerAction()
                {
                    Uuid = serverResponse.Uuid,
                    ExternalId = serverResponse.ExternalId,
                    Method = method,
                    Parameters = parameters
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while preparing command: {serverResponse.CommandName}");
                return;
            }

            ThreadPool.QueueUserWorkItem(async _ =>
            {
                try
                {
                    await _terminal.AddServerActionAsync(serverAction);

                    using var scope = _serviceProvider.CreateScope();
                    var unitofWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    await unitofWork.SaveChangesAsync(); //Publish domain events

                }
                catch (TargetInvocationException ex) when (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, $"Error while executing command: {serverResponse.CommandName}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error while executing command: {serverResponse.CommandName}");
                }
            });
        }
    }
}