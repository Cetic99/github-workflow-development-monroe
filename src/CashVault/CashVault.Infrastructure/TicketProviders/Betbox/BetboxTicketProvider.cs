using System.Text;
using System.Text.Json;
using CashVault.Application.Interfaces;
using CashVault.DeviceDriver.Common;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Infrastructure.TicketProviders.Betbox.Dtos;
using Microsoft.Extensions.Options;

namespace CashVault.Infrastructure.TicketProviders.Betbox;

internal class BetboxTicketProvider : ITicketProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITerminal _terminal;
    private readonly LocalDevEnvOptions _localDevEnv;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public BetboxTicketProvider(IHttpClientFactory httpClientFactory,
                          ITerminal terminal,
                          IOptions<LocalDevEnvOptions> localDevEnvOptions)
    {
        _httpClientFactory = httpClientFactory;
        _terminal = terminal;
        _localDevEnv = localDevEnvOptions?.Value ?? new LocalDevEnvOptions();
    }

    public async Task<bool> RedeemTicketAck(string barcode, Guid? id)
    {
        if (_localDevEnv.Enabled)
        {
            // Simulate a delay for the mock environment
            await Task.Delay(500);
            return true;
        }

        if (_terminal.ServerConfiguration.IsEnabled == false)
        {
            throw new InvalidOperationException("Server communication is disabled.");
        }

        var client = _httpClientFactory.CreateClient();

        var payload = new BetboxRequestDto(barcode, id);
        var serializedPayload = JsonSerializer.Serialize(payload, _jsonOptions);

        var uri = new Uri(_terminal.ServerConfiguration.ServerUrl);
        string baseAddress = $"{uri.Scheme}://{uri.Authority}";

        var request = new HttpRequestMessage(HttpMethod.Post, $"{baseAddress}/api/betbox/ticket_ack")
        {
            Content = new StringContent(serializedPayload, Encoding.UTF8, "application/json")
        };

        var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return true;
    }

    public async Task<Application.Features.TicketFeatures.Queries.RedeemTicketDto?> RedeemTicket(string barcode, Guid? id)
    {
        if (_localDevEnv.Enabled)
        {
            // Simulate a delay for the mock environment
            await Task.Delay(500);
            return new Application.Features.TicketFeatures.Queries.RedeemTicketDto
            {
                Barcode = barcode,
                TotalAmount = 10,
                DateCreated = DateTime.UtcNow,
                Currency = Domain.ValueObjects.Currency.BAM,
                IsValid = true
            };
        }

        if (_terminal.ServerConfiguration.IsEnabled == false)
        {
            return null;
        }

        var client = _httpClientFactory.CreateClient();

        var uri = new Uri(_terminal.ServerConfiguration.ServerUrl);

        string baseAddress = $"{uri.Scheme}://{uri.Authority}";
        var request = new HttpRequestMessage(HttpMethod.Get, $"{baseAddress}/api/betbox/ticket/{barcode}/id/{id}/info");

        var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var stringResp = await response.Content.ReadAsStringAsync();
        var redeemTicket = JsonSerializer.Deserialize<Dtos.RedeemTicketDto>(stringResp, _jsonOptions);

        var result = new Application.Features.TicketFeatures.Queries.RedeemTicketDto();

        if (redeemTicket != null)
        {
            result.Barcode = redeemTicket.Barcode;
            result.IsValid = redeemTicket.Status == (int)BetboxTicketStatus.Active;
            result.Currency = Domain.ValueObjects.Currency.BAM;
            result.DateCreated = redeemTicket.DateCreated;
            result.DateUsed = redeemTicket.DateUsed;
            result.TotalAmount = redeemTicket.TotalAmount;
        }

        return result;
    }

    public async Task<bool> RedeemTicketNack(string barcode, Guid? id)
    {
        if (_localDevEnv.Enabled)
        {
            // Simulate a delay for the mock environment
            await Task.Delay(500);
            return true;
        }

        if (_terminal.ServerConfiguration.IsEnabled == false)
        {
            throw new InvalidOperationException("Server communication is disabled.");
        }

        var client = _httpClientFactory.CreateClient();

        var payload = new BetboxRequestDto(barcode, id);
        var serializedPayload = JsonSerializer.Serialize(payload, _jsonOptions);

        var uri = new Uri(_terminal.ServerConfiguration.ServerUrl);
        string baseAddress = $"{uri.Scheme}://{uri.Authority}";

        var request = new HttpRequestMessage(HttpMethod.Post, $"{baseAddress}/api/betbox/ticket_nack")
        {
            Content = new StringContent(serializedPayload, Encoding.UTF8, "application/json")
        };

        var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return true;
    }
}
