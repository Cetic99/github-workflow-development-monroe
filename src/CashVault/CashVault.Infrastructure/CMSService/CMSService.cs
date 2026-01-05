using System.Text;
using System.Text.Json;
using CashVault.Application.Common.Models.CMSResponses;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using CashVault.Domain.Common.Events;
using CashVault.Infrastructure.CMSService.Dtos;
using Microsoft.Extensions.Logging;

namespace CashVault.Infrastructure.CMSService
{
    internal class CMSService : ICMSService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CMSService> _logger;
        private readonly ISessionService _sessionService;
        private readonly JsonSerializerOptions _deserializeOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
        private readonly JsonSerializerOptions _serializeOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public CMSService(IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory, ILogger<CMSService> logger, ISessionService sessionService)
        {
            _unitOfWork = unitOfWork;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _sessionService = sessionService;
        }

        public async Task<TicketRedemptionResponseModel?> RedeemTicket(string barcode)
        {
            var config = await GetCMSConfiguration();
            if (!config.CasinoManagementSystem)
            {
                return null;
            }

            var data = new TicketRedemptionRequestDto(type: CMSCommands.TicketRedemption.Code, dateTime: DateTime.UtcNow, language: _sessionService.Language ?? "en", machineName: config.DeviceId, secretKey: config.SecretKey, barcode);

            var ticketRedemptionResponse = await SendToCMS<TicketRedemptionResponseModel>(config.Url!, data, config.TimeoutInSeconds);

            if (ticketRedemptionResponse != null)
            {
                ticketRedemptionResponse.IsSuccessful = ticketRedemptionResponse.ResponseCode == CMSResponseCodes.Valid;
            }

            return ticketRedemptionResponse;
        }

        public async Task<bool> CompleteRedeemTicket(string barcode)
        {
            var config = await GetCMSConfiguration();
            if (!config.CasinoManagementSystem)
            {
                return false;
            }

            var data = new TicketRedemptionAckNackRequestDto(type: CMSCommands.TicketRedemptionAck.Code, dateTime: DateTime.UtcNow, machineName: config.DeviceId, secretKey: config.SecretKey, barcode);

            var ticketRedemptionAck = await SendToCMS<TicketRedemptionAckNackResponseDto>(config.Url!, data, config.TimeoutInSeconds);

            if (ticketRedemptionAck != null && ticketRedemptionAck.ResponseCode == CMSResponseCodes.Valid)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> FailTicketRedemption(string barcode)
        {
            var config = await GetCMSConfiguration();
            if (!config.CasinoManagementSystem)
            {
                return false;
            }

            var data = new TicketRedemptionAckNackRequestDto(type: CMSCommands.TicketRedemptionNack.Code, dateTime: DateTime.UtcNow, machineName: config.DeviceId, secretKey: config.SecretKey, barcode);

            var ticketRedemptionNack = await SendToCMS<TicketRedemptionAckNackResponseDto>(config.Url!, data, config.TimeoutInSeconds);

            if (ticketRedemptionNack != null && ticketRedemptionNack.ResponseCode == CMSResponseCodes.Valid)
            {
                return true;
            }

            return false;
        }

        public async Task<TicketPrintResponseModel?> RequestTicketPrinting(decimal amount)
        {
            var config = await GetCMSConfiguration();
            if (!config.CasinoManagementSystem)
            {
                return null;
            }

            var data = new TicketPrintRequestDto(type: CMSCommands.TicketPrintRequest.Code, dateTime: DateTime.UtcNow, language: _sessionService.Language ?? "en", machineName: config.DeviceId, secretKey: config.SecretKey, amount);

            var ticketPrintResponse = await SendToCMS<TicketPrintResponseModel>(config.Url!, data, config.TimeoutInSeconds);

            if (ticketPrintResponse != null)
            {
                ticketPrintResponse.IsSuccessful = ticketPrintResponse.ResponseCode == CMSResponseCodes.Valid;
            }

            return ticketPrintResponse;
        }


        public async Task<bool> CompleteTicketPrinting(string barcode)
        {
            var config = await GetCMSConfiguration();
            if (!config.CasinoManagementSystem)
            {
                return false;
            }

            var data = new TicketPrintCompleteFailRequestDto(type: CMSCommands.TicketPrintComplete.Code, dateTime: DateTime.UtcNow, machineName: config.DeviceId, secretKey: config.SecretKey, barcode);

            var ticketCompleteResponse = await SendToCMS<TicketPrintCompleteFailResponseDto>(config.Url!, data, config.TimeoutInSeconds);

            if (ticketCompleteResponse != null && ticketCompleteResponse.ResponseCode == CMSResponseCodes.Valid)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> FailTicketPrinting(string barcode)
        {
            var config = await GetCMSConfiguration();
            if (!config.CasinoManagementSystem)
            {
                return false;
            }

            var data = new TicketPrintCompleteFailRequestDto(type: CMSCommands.TicketPrintFailed.Code, dateTime: DateTime.UtcNow, machineName: config.DeviceId, secretKey: config.SecretKey, barcode: barcode);

            var ticketFailResponse = await SendToCMS<TicketPrintCompleteFailResponseDto>(config.Url!, data, config.TimeoutInSeconds);

            if (ticketFailResponse != null && ticketFailResponse.ResponseCode == CMSResponseCodes.Valid)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> SendEvent(BaseEvent @event)
        {
            var config = await GetCMSConfiguration();
            if (!config.CasinoManagementSystem)
            {
                return false;
            }

            var data = new EventRequestDto(type: CMSCommands.Event.Code, dateTime: DateTime.UtcNow, machineName: config.DeviceId, secretKey: config.SecretKey, @event);

            var eventResponse = await SendToCMS<EventResponseDto>(config.Url!, data, config.TimeoutInSeconds);

            if (eventResponse != null && eventResponse.ResponseCode == CMSResponseCodes.Valid)
            {
                return true;
            }

            _logger.LogError($"Failed to send event [uuid: {@event.Guid}] to CMS");

            return false;
        }

        public async Task<bool> SendTransactionEvent(TransactionEvent transactionEvent, DispenserBillCountStatus dispenserBillCountStatus, BillTicketAcceptorStackerStatus acceptorMoneyStatus)
        {
            var config = await GetCMSConfiguration();
            if (!config.CasinoManagementSystem)
            {
                return false;
            }

            var data = new TransactionRequestDto(CMSCommands.Transaction.Code, DateTime.UtcNow, config.DeviceId, secretKey: config.SecretKey, transactionEvent, dispenserBillCountStatus, acceptorMoneyStatus);

            var transactionResponse = await SendToCMS<TransactionResponseDto>(config.Url!, data, config.TimeoutInSeconds);

            if (transactionResponse != null && transactionResponse.ResponseCode == CMSResponseCodes.Valid)
            {
                return true;
            }

            _logger.LogError($"Failed to send transaction [uuid: {transactionEvent.Guid}] to CMS");

            return false;
        }

        private async Task<OnlineIntegrationsConfiguration> GetCMSConfiguration()
        {
            var onlineIntegrations = await _unitOfWork.TerminalRepository.GetCurrentOnlineIntegrationsConfigurationAsync();

            if (onlineIntegrations == null)
            {
                return new OnlineIntegrationsConfiguration();
            }

            return onlineIntegrations;
        }

        private async Task<T?> SendToCMS<T>(string url, object data, int timeoutInSeconds)
        {
            if (string.IsNullOrEmpty(url))
            {
                _logger.LogError("CMS URL is not set.");
                return default;
            }

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(timeoutInSeconds);

            string jsonData = JsonSerializer.Serialize(data, _serializeOptions);
            _logger.LogDebug($"Sending request to CMS. URL: {url}, Payload: {jsonData}");

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            try
            {
                response = await client.PostAsync(url, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send request to CMS. URL: {url}, Payload: {jsonData}");
                return default;
            }

            string responseData;
            try
            {
                responseData = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read CMS response.");
                return default;
            }

            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Received successful response from CMS. Status Code: {response.StatusCode}, Response: {responseData}");
                try
                {
                    return JsonSerializer.Deserialize<T>(responseData, _deserializeOptions);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"CMS response deserialization failed, message: {ex?.Message}, response data: {responseData}");
                    return default;
                }
            }
            else
            {
                _logger.LogError($"Request failed with status code {response.StatusCode}. Response: {responseData}");
                return default;
            }
        }
    }
}
