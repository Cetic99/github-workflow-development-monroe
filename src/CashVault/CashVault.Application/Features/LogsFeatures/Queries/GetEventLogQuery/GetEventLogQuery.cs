using CashVault.Application.Interfaces;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common;
using CashVault.Domain.Common.Events;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class GetEventLogQueryValidator : AbstractValidator<GetEventLogQuery>
    {
        public GetEventLogQueryValidator()
        {
            RuleFor(x => x.Page).NotNull().WithMessage("Page is required.");
            RuleFor(x => x.PageSize).NotEmpty().WithMessage("PageSize is required.");
        }
    }

    public record GetEventLogQuery : IRequest<EventLogDto>
    {
        public int Page { get; init; }
        public int PageSize { get; init; }
        public string? Message { get; init; }
        public string? Type { get; init; }
        public string? DeviceType { get; init; }
        public string? Name { get; init; }
        public DateTime? TimestampFrom { get; init; }
        public DateTime? TimestampTo { get; init; }
    }

    internal sealed class GetEventLogQueryHandler : IRequestHandler<GetEventLogQuery, EventLogDto>
    {
        private readonly IEventLogRepository _db;
        private readonly ITerminal _terminal;
        private readonly IRegionalService _regionalService;

        public GetEventLogQueryHandler(IEventLogRepository db, ITerminal terminal, IRegionalService regionalService)
        {
            _db = db;
            _terminal = terminal;
            _regionalService = regionalService;
        }

        public async Task<EventLogDto> Handle(GetEventLogQuery request, CancellationToken cancellationToken)
        {
            var logs = await _db.GetEventLogsAsync(request.Page, request.PageSize, request.Message, request.Name, Enumeration.GetByCode<EventLogType>(request?.Type), Enumeration.GetByCode<DeviceType>(request?.DeviceType), request.TimestampFrom, request.TimestampTo);

            var result = new EventLogDto(logs.Page, logs.PageSize, logs.TotalCount);

            if (logs == null || !logs.Data.Any())
            {
                return result;
            }

            var timeZone = _terminal.LocalTimeZone;

            foreach (var log in logs.Data)
            {
                result.Items.Add(new LogItemDto()
                {
                    Id = log.Id,
                    Timestamp = _regionalService.ConvertToUserTimeZone(log.Created, timeZone),
                    Message = log.Message,
                    Type = log.Type,
                    Name = log.EventName,
                    DeviceType = log is DeviceEvent deviceEvent ? deviceEvent.DeviceType : null
                });
            }

            return result;
        }
    }
}