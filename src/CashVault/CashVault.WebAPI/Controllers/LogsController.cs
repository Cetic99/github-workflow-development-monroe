using CashVault.Application.Features.DeviceFeatures.Queries;
using CashVault.Application.Features.LogsFeatures.Queries;
using CashVault.Domain.Aggregates.OperatorAggregate;
using CashVault.WebAPI.Common.CustomAttributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CashVault.WebAPI.Controllers
{
    [Authorize]
    [Route("api/logs")]
    [ApiController]
    public class LogsController : BaseController
    {
        [HttpGet("event")]
        [HasPermission(PermissionEnum.Logs)]
        public async Task<EventLogDto> GetEventLogs(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] string? message,
            [FromQuery] string? name,
            [FromQuery] string? eventType,
            [FromQuery] string? deviceType,
            [FromQuery] DateTime? timestampFrom,
            [FromQuery] DateTime? timestampTo)
        {
            return await Mediator.Send(new GetEventLogQuery()
            {
                Page = page,
                PageSize = pageSize,
                Message = message,
                Type = eventType,
                DeviceType = deviceType,
                Name = name,
                TimestampFrom = timestampFrom,
                TimestampTo = timestampTo
            });
        }

        [HttpGet("transactions")]
        [HasPermission(PermissionEnum.Logs)]
        public async Task<TransactionLogsDto> GetTransactionLogs
            ([FromQuery] int page,
             [FromQuery] int pageSize,
             [FromQuery] DateTime? processingStartedDateFrom,
             [FromQuery] DateTime? processingStartedDateTo,
             [FromQuery] DateTime? processingEndedDateFrom,
             [FromQuery] DateTime? processingEndedDateTo,
             [FromQuery] decimal? amountFrom,
             [FromQuery] decimal? amountTo,
             [FromQuery] decimal? amountRequestedFrom,
             [FromQuery] decimal? amountRequestedTo,
             [FromQuery] int? typeId,
             [FromQuery] string? kind,
             [FromQuery] int? statusId)
        {
            return await Mediator.Send(new GetTransactionLogQuery()
            {
                Page = page,
                PageSize = pageSize,
                ProcessingEndedDateFrom = processingEndedDateFrom,
                ProcessingEndedDateTo = processingEndedDateTo,
                ProcessingStartedDateFrom = processingStartedDateFrom,
                ProcessingStartedDateTo = processingStartedDateTo,
                AmountFrom = amountFrom,
                AmountTo = amountTo,
                AmountRequestedFrom = amountRequestedFrom,
                AmountRequestedTo = amountRequestedTo,
                TypeId = typeId,
                Kind = kind,
                StatusId = statusId
            });
        }

        [HttpGet("transaction-details/{id}")]
        [HasPermission(PermissionEnum.Logs)]
        public async Task<TransactionLogDetailsDto> GetTransactionLogDetails([FromRoute] int id)
        {
            return await Mediator.Send(new GetTransactionLogDetailsQuery()
            {
                Id = id
            });
        }

        [HttpGet("money-status-transactions")]
        [HasPermission(PermissionEnum.Logs)]
        public async Task<MoneyStatusTransactionLogsDto> GetMoneyStatusTransactions([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] int? typeId, int? kindId, decimal? amountFrom, decimal? amountTo, DateTime? timestampFrom, DateTime? timestampTo)
        {
            return await Mediator.Send(new GetMoneyStatusTransactionQuery()
            {
                Page = page,
                PageSize = pageSize,
                TimestampFrom = timestampFrom,
                TimestampTo = timestampTo,
                AmountFrom = amountFrom,
                AmountTo = amountTo,
                TypeId = typeId,
            });
        }

        [HttpGet("money-status-transaction/{id}/details")]
        [HasPermission(PermissionEnum.Logs)]
        public async Task<MoneyTransactionLogDetailsDto> GetMoneyStatusTransactionLogDetails([FromRoute] int id)
        {
            return await Mediator.Send(new GetMoneyTransactionLogDetailsQuery()
            {
                Id = id
            });
        }

        [HttpGet("tickets")]
        [HasPermission(PermissionEnum.Logs)]
        public async Task<TicketLogsDto> GetTicketLogs
            ([FromQuery] int page,
             [FromQuery] int pageSize,
             [FromQuery] string? barcode,
             [FromQuery] string? number,
             [FromQuery] decimal? amountFrom,
             [FromQuery] decimal? amountTo,
             [FromQuery] DateTime? printingRequestedAtFrom,
             [FromQuery] DateTime? printingRequestedAtTo,
             [FromQuery] DateTime? printingCompletedAtFrom,
             [FromQuery] DateTime? printingCompletedAtTo,
             [FromQuery] int? daysValidFrom,
             [FromQuery] int? daysValidTo,
             [FromQuery] int? typeId,
             [FromQuery] bool? isLocal,
             [FromQuery] bool? isPrinted,
             [FromQuery] bool? isRedeemed,
             [FromQuery] bool? isStacked,
             [FromQuery] bool? isExpired)
        {
            return await Mediator.Send(new GetTicketLogQuery()
            {
                Page = page,
                PageSize = pageSize,
                Barcode = barcode,
                Number = number,
                AmountFrom = amountFrom,
                AmountTo = amountTo,
                PrintingCompletedAtFrom = printingCompletedAtFrom,
                PrintingCompletedAtTo = printingCompletedAtTo,
                PrintingRequestedAtFrom = printingRequestedAtFrom,
                PrintingRequestedAtTo = printingRequestedAtTo,
                DaysValidFrom = daysValidFrom,
                DaysValidTo = daysValidTo,
                TypeId = typeId,
                IsLocal = isLocal,
                IsPrinted = isPrinted,
                IsRedeemed = isRedeemed,
                IsStacked = isStacked,
                IsExpired = isExpired
            });
        }

        [HttpGet("ticket-details/{id}")]
        [HasPermission(PermissionEnum.Logs)]
        public async Task<TicketLogDetailsDto> GetTicketLogDetails([FromRoute] int id)
        {
            return await Mediator.Send(new GetTicketLogDetailsQuery()
            {
                Id = id
            });
        }

        [HttpGet("fail")]
        public async Task<FailLogDto> GetFailLogs(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] string? message,
            [FromQuery] string? name,
            [FromQuery] string? deviceType,
            [FromQuery] DateTime? timestampFrom,
            [FromQuery] DateTime? timestampTo)
        {
            return await Mediator.Send(new GetFailLogQuery()
            {
                Page = page,
                PageSize = pageSize,
                Message = message,
                Name = name,
                DeviceType = deviceType,
                TimestampFrom = timestampFrom,
                TimestampTo = timestampTo
            });
        }

        [HttpGet("life-meter")]
        [HasPermission(PermissionEnum.Logs)]
        public async Task<LifeMeterDto> GetLifeMeter()
        {
            return await Mediator.Send(new GetLifeMeterQuery());
        }
    }
}
