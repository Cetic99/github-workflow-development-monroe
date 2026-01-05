using System.Text.Json;
using CMSMock.Constants;
using CMSMock.Dtos;
using CMSMock.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CMSMock.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CMSController : Controller
    {
        private readonly ILogger<CMSController> _logger;
        private readonly RequestVerifier _requestVerifier;
        private readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public CMSController(ILogger<CMSController> logger, RequestVerifier requestVerifier)
        {
            _logger = logger;
            _requestVerifier = requestVerifier;
        }

        [HttpPost("/api/v1/cms/integration")]
        public IActionResult Integration(JsonDocument data)
        {
            try
            {

                data.RootElement.TryGetProperty("type", out JsonElement type1);
                data.RootElement.TryGetProperty("Type", out JsonElement type2);

                JsonElement type = type1;
                if (type.ValueKind == JsonValueKind.Undefined)
                {
                    type = type2;
                }

                if (type.ValueKind == JsonValueKind.Undefined)
                {
                    return BadRequest("Type not found");
                }

                _logger.LogInformation($"Received integration request: {type}");

                if (type.GetString() == Commands.HealthCheck)
                {
                    return Ok("OK");
                }

                if (type.GetString() == Commands.TicketRedemption)
                {
                    var ticketRedemption = JsonSerializer.Deserialize<TicketRedemptionRequestDto>(data.RootElement.ToString());

                    return Ok(new TicketRedemptionResponseModel()
                    {
                        Type = Commands.TicketRedemption,
                        Language = "en",
                        MachineName = ticketRedemption.MachineName,
                        Barcode = ticketRedemption.Barcode,
                        ResponseCode = MessageCodes.Valid,
                        Amount = 10.00m,
                        AmountWithTaxes = 10.00m,
                        Taxes = 0,
                        Reason = null
                    });
                }
                else if (type.GetString() == Commands.TicketRedemptionAck)
                {
                    var ticketRedemptionAck = JsonSerializer.Deserialize<TicketRedemptionAckNackRequestDto>(data.RootElement.ToString());

                    return Ok(new TicketRedemptionAckNackResponseDto()
                    {
                        Type = Commands.TicketRedemptionAck,
                        MachineName = ticketRedemptionAck.MachineName,
                        Barcode = ticketRedemptionAck.Barcode,
                        ResponseCode = MessageCodes.Valid,
                    });
                }
                else if (type.GetString() == Commands.TicketRedemptionNack)
                {
                    var ticketRedemptionNack = JsonSerializer.Deserialize<TicketRedemptionAckNackRequestDto>(data.RootElement.ToString());

                    return Ok(new TicketRedemptionAckNackResponseDto()
                    {
                        Type = Commands.TicketRedemptionNack,
                        MachineName = ticketRedemptionNack.MachineName,
                        Barcode = ticketRedemptionNack.Barcode,
                        ResponseCode = MessageCodes.Valid,
                    });
                }
                else if (type.GetString() == Commands.TicketPrintRequest)
                {
                    var ticketPrintRequest = JsonSerializer.Deserialize<TicketPrintRequestDto>(data.RootElement.ToString());

                    return Ok(new TicketPrintResponseModel()
                    {
                        Type = Commands.TicketPrintRequest,
                        Language = "en",
                        MachineName = ticketPrintRequest.MachineName,
                        ResponseCode = MessageCodes.Valid,
                        Barcode = BarcodeGenerator.GenerateBarcode(),
                        Amount = ticketPrintRequest.Amount,
                        Validity = 30,
                        AmountText = $"BAM {ticketPrintRequest.Amount}",
                        AmountInWords = "Amount BAM",
                        Title = "Cash Vault",
                        Location = "Casino",
                        Address1 = "Some address",
                        Address2 = "Banja Luka",
                        DatePrint = ticketPrintRequest.DateTime
                    });
                }
                else if (type.GetString() == Commands.TicketPrintComplete)
                {
                    var ticketPrintComplete = JsonSerializer.Deserialize<TicketPrintCompleteRequestDto>(data.RootElement.ToString());

                    return Ok(new TicketPrintCompleteFailResponseDto()
                    {
                        Type = Commands.TicketPrintComplete,
                        DateTime = DateTime.UtcNow,
                        MachineName = ticketPrintComplete.MachineName,
                        Barcode = ticketPrintComplete.Barcode,
                        ResponseCode = MessageCodes.Valid,
                    });
                }
                else if (type.GetString() == Commands.TicketPrintFailed)
                {
                    var ticketPrintFailed = Deserialize<TicketPrintCompleteRequestDto>(data.RootElement.ToString());

                    return Ok(new TicketPrintCompleteFailResponseDto()
                    {
                        Type = Commands.TicketPrintFailed,
                        DateTime = DateTime.UtcNow,
                        MachineName = ticketPrintFailed.MachineName,
                        Barcode = ticketPrintFailed.Barcode,
                        ResponseCode = MessageCodes.Valid,
                    });
                }
                else if (type.GetString() == Commands.Transaction)
                {
                    var transactionRequest = Deserialize<TransactionRequestDto>(data.RootElement.ToString());

                    return Ok(new TransactionResponseDto()
                    {
                        Type = Commands.Transaction,
                        MachineName = transactionRequest.MachineName,
                        ResponseCode = MessageCodes.Valid,
                    });
                }
                else if (type.GetString() == Commands.Event)
                {
                    var eventRequest = Deserialize<EventRequestDto>(data.RootElement.ToString());

                    return Ok(new EventResponseDto()
                    {
                        Type = Commands.Event,
                        MachineName = eventRequest.MachineName,
                        ResponseCode = MessageCodes.Valid,
                    });
                }

                return BadRequest("Type not supported");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/api/v1/backbone/events")]
        public IActionResult Events(JsonDocument data)
        {
            _logger.LogInformation($"Received event: {JsonSerializer.Serialize(data)}");


            //return BadRequest("Type not supported");
            return Ok(new
            {
                ResponseCode = MessageCodes.Valid,
                DateTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                MachineName = "kanc",
                Secret = "xxxxxx",
            });
        }

        [HttpPost("/api/v1/backbone/heartbeat")]
        public async Task<IActionResult> HeartBeat(JsonDocument data)
        {
            _logger.LogInformation($"Received event: {JsonSerializer.Serialize(data)}");

            if (!Request.Headers.TryGetValue("X-Request-Signing-Algorithm", out var signAlg))
            {
                return BadRequest("Missing signing algorithm header.");
            }

            if (signAlg != "RSA")
            {
                return BadRequest("Invalid signing algorithm.");
            }

            Request.Headers.TryGetValue("X-Request-Hash-Alg", out var hashAlg);

            if (hashAlg != "SHA-512")
            {
                return BadRequest("Invalid hash algorithm.");
            }

            if (!Request.Headers.TryGetValue("X-Request-Signature", out var signature) ||
                !_requestVerifier.VerifyRequest(data, signature))
            {
                return Unauthorized("Invalid signature.");
            }

            data.RootElement.TryGetProperty("timestamp", out JsonElement timestamp);

            if (timestamp.ValueKind == JsonValueKind.Undefined) return BadRequest("Timestamp not found");

            if (string.IsNullOrEmpty(timestamp.GetString()) || !IsTimestampValid(timestamp.GetString()))
            {
                return BadRequest("Invalid timestamp.");
            }

            var response = new ServerResponse()
            {
                CommandName = "PrintMessage",
                Params =
                [
                    new CommandParam("deviceId", "001")
                ]
            };

            return Ok(response);
        }

        private T? Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, jsonOptions);
        }

        //Check if the timestamp is within 5 seconds of the current time
        private bool IsTimestampValid(string timestamp)
        {
            if (!DateTime.TryParse(timestamp, out var requestTime))
                return false;

            return (DateTime.UtcNow - requestTime).TotalSeconds <= 5;
        }
    }
}
