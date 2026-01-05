using CashVault.Application.Features.ParcelLockerFeatures.Models;
using CashVault.Application.Features.ParcelLockerFeatures.QueryModels;
using CashVault.Application.Interfaces;
using CashVault.DeviceDriver.Common;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using CashVault.Infrastructure.PostalServiceProviders.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace CashVault.Infrastructure.PostalServiceProviders;

public class PostalServiceProvider : IPostalServiceProvider
{

    private static readonly Random random = new();
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly LocalDevEnvOptions _localDevEnv;
    private readonly ITerminal _terminal;
    private ILogger<PostalServiceProvider> _logger;

    public PostalServiceProvider(
        IHttpClientFactory httpClientFactory,
        IOptions<LocalDevEnvOptions> localDevEnvOptions,
        ITerminal terminal,
        ILogger<PostalServiceProvider> logger)
    {
        _httpClientFactory = httpClientFactory;
        _localDevEnv = localDevEnvOptions?.Value ?? new LocalDevEnvOptions();
        _terminal = terminal;
        _logger = logger;
    }


    public async Task<ShipmentModel?> FetchShipment(
        string postalService,
        string? barcode,
        long lockerAccessCode)
    {
        if (_localDevEnv.Enabled)
        {
            // Simulate a delay for the mock environment
            await Task.Delay(500);

            if (lockerAccessCode <= 0)
                lockerAccessCode = GenerateLong();

            if (lockerAccessCode % 3 == 0)
                return new ShipmentModel(
                    barcode: barcode ?? GenerateBarcode(),
                    registrationNumber: GenerateLong(),
                    lockerAccessCode: lockerAccessCode,
                    amount: lockerAccessCode % 2 == 0 ? 0m : 199.95m,
                    parcelLockerSize: ParcelLockerSize.Small.Code,
                    recieverPhoneNumber: "+387111222",
                    expirationDateNumber: DateTime.Now.AddDays(5),
                    postalService: postalService);

            if (lockerAccessCode % 3 == 1)
                return new ShipmentModel(
                    barcode: barcode ?? GenerateBarcode(),
                    registrationNumber: GenerateLong(),
                    lockerAccessCode: lockerAccessCode,
                    amount: lockerAccessCode % 2 == 0 ? 0m : 199.95m,
                    parcelLockerSize: ParcelLockerSize.Medium.Code,
                    recieverPhoneNumber: "+387111222",
                    expirationDateNumber: DateTime.Now.AddDays(5),
                    postalService: postalService);

            return new ShipmentModel(
                barcode: barcode ?? GenerateBarcode(),
                registrationNumber: GenerateLong(),
                lockerAccessCode: lockerAccessCode,
                amount: lockerAccessCode % 2 == 0 ? 0m : 199.95m,
                parcelLockerSize: ParcelLockerSize.Large.Code,
                recieverPhoneNumber: "+387111222",
                expirationDateNumber: DateTime.Now.AddDays(5),
                postalService: postalService);
        }

        if (_terminal.ServerConfiguration.IsEnabled == false)
            throw new InvalidOperationException("Server communication is disabled.");

        var client = _httpClientFactory.CreateClient();

        var uri = new Uri(_terminal.ServerConfiguration.ServerUrl);
        string baseAddress = $"{uri.Scheme}://{uri.Authority}";

        string shipmentId = barcode ?? string.Empty;

        if (string.IsNullOrWhiteSpace(barcode))
            shipmentId = lockerAccessCode.ToString();

        var request = new HttpRequestMessage(HttpMethod.Get, $"{baseAddress}/api/monroe_backoffice/parcel_locker/{postalService}/shipment/{shipmentId}");
        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch shipment data from postal service provider. Status code: {StatusCode}", response.StatusCode);
            return null;
        }

        try
        {
            string responseContentJson = await response.Content.ReadAsStringAsync();
            ShipmentModel? responseContent = JsonSerializer.Deserialize<ShipmentModel>(responseContentJson, _jsonOptions);

            return responseContent;
        }
        catch (Exception)
        {
            _logger.LogError("Failed to deserialize shipment data from postal service provider.");
            return null;
        }
    }

    public async Task<CourierModel?> ProcessCourierBarcode(string postalService, string barcode)
    {
        if (_localDevEnv.Enabled)
        {
            await Task.Delay(500);

            return new CourierModel()
            {
                Id = barcode,
                Name = $"{postalService} Courier",
                PostalService = postalService
            };
        }

        if (_terminal.ServerConfiguration.IsEnabled == false)
            throw new InvalidOperationException("Server communication is disabled.");

        var client = _httpClientFactory.CreateClient();

        var uri = new Uri(_terminal.ServerConfiguration.ServerUrl);
        string baseAddress = $"{uri.Scheme}://{uri.Authority}";

        var payload = new VerifyCourierRequest()
        {
            PostalService = postalService,
            Barcode = barcode
        };
        var serializedPayload = JsonSerializer.Serialize(payload, _jsonOptions);

        var request = new HttpRequestMessage(HttpMethod.Post, $"{baseAddress}/api/monroe_backoffice/parcel_locker/verify_courier")
        {
            Content = new StringContent(serializedPayload, Encoding.UTF8, "application/json")
        };
        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch courier data from postal service provider. Status code: {StatusCode}", response.StatusCode);
            return null;
        }

        try
        {
            string responseContentJson = await response.Content.ReadAsStringAsync();
            CourierModel? responseContent = JsonSerializer.Deserialize<CourierModel>(responseContentJson, _jsonOptions);

            return responseContent;
        }
        catch (Exception)
        {
            _logger.LogError("Failed to deserialize courier data from postal service provider.");
            return null;
        }
    }

    public async Task<List<ShipmentDto>> GetPendingShipments(string postalService)
    {
        if (_localDevEnv.Enabled)
        {
            await Task.Delay(500);

            return [];
        }

        if (_terminal.ServerConfiguration.IsEnabled == false)
            throw new InvalidOperationException("Server communication is disabled.");

        var client = _httpClientFactory.CreateClient();

        var uri = new Uri(_terminal.ServerConfiguration.ServerUrl);
        string baseAddress = $"{uri.Scheme}://{uri.Authority}";

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"{baseAddress}/api/monroe_backoffice/parcel_locker/shipments?status={ShipmentStatus.Announced.Code}&postalService={postalService}");
        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch pending shipments from postal service provider. Status code: {StatusCode}", response.StatusCode);
            return [];
        }

        try
        {
            string responseContentJson = await response.Content.ReadAsStringAsync();
            List<ShipmentDto>? responseContent = JsonSerializer.Deserialize<List<ShipmentDto>>(responseContentJson, _jsonOptions);

            return responseContent;
        }
        catch (Exception)
        {
            _logger.LogError("Failed to deserialize pending shipments data from postal service provider.");
            return [];
        }
    }

    public async Task<bool> UpdateShipmentStatus(ParcelLockerShipment shipment, ShipmentStatus status)
    {
        if (_localDevEnv.Enabled)
        {
            await Task.Delay(500);

            return true;
        }

        if (_terminal.ServerConfiguration.IsEnabled == false)
            throw new InvalidOperationException("Server communication is disabled.");

        var client = _httpClientFactory.CreateClient();

        var uri = new Uri(_terminal.ServerConfiguration.ServerUrl);
        string baseAddress = $"{uri.Scheme}://{uri.Authority}";

        var payload = new ChangeShipmentStatusRequest()
        {
            PostalService = shipment.PostalService,
            Barcode = shipment.Barcode,
            StatusCode = status.Code
        };
        var serializedPayload = JsonSerializer.Serialize(payload, _jsonOptions);

        var request = new HttpRequestMessage(HttpMethod.Put, $"{baseAddress}/api/monroe_backoffice/parcel_locker/shipment/{shipment.Barcode}/change_status")
        {
            Content = new StringContent(serializedPayload, Encoding.UTF8, "application/json")
        };
        var response = await client.SendAsync(request);

        // TODO: Hanlde errors
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch courier data from postal service provider. Status code: {StatusCode}", response.StatusCode);
            return false;
        }

        return true;
    }

    // TODO
    public async Task<ShipmentDetailsModel?> CreateShipmentAsync(CreateShipmentModel data)
    {
        // TODO: Call backoffice
        await Task.Delay(500);

        return new ShipmentDetailsModel()
        {
            Barcode = GenerateBarcode(),
            LockerAccessCode = GenerateLong(),
            RegistrationNumber = GenerateLong()
        };
    }

    private static long GenerateLong()
    {
        long number = random.Next(1000000, 10000000);
        return number;
    }

    private static string GenerateBarcode(int length = 12)
    {
        char[] digits = new char[length];

        for (int i = 0; i < length; i++)
            digits[i] = (char)('0' + random.Next(0, 10));

        return new string(digits);
    }
}
