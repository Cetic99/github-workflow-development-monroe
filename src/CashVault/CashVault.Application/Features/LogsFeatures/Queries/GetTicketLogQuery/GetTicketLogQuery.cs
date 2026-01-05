using CashVault.Application.Interfaces;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.TicketAggregate;
using CashVault.Domain.Common;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class GetTicketLogQueryValidator : AbstractValidator<GetTicketLogQuery>
    {
        public GetTicketLogQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThan(-1);
            RuleFor(x => x.PageSize).NotEmpty();
        }
    }

    public record GetTicketLogQuery : IRequest<TicketLogsDto>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string? Barcode { get; set; }
        public string? Number { get; set; }
        public string? Guid { get; set; }
        public decimal? AmountFrom { get; set; }
        public decimal? AmountTo { get; set; }
        public DateTime? PrintingRequestedAtFrom { get; set; }
        public DateTime? PrintingRequestedAtTo { get; set; }
        public DateTime? PrintingCompletedAtFrom { get; set; }
        public DateTime? PrintingCompletedAtTo { get; set; }
        public int? TypeId { get; set; }
        public int? DaysValidFrom { get; set; }
        public int? DaysValidTo { get; set; }
        public bool? IsLocal { get; set; }
        public bool? IsPrinted { get; set; }
        public bool? IsRedeemed { get; set; }
        public bool? IsStacked { get; set; }
        public bool? IsExpired { get; set; }
    }

    internal sealed class GetTicketLogQueryHandler : IRequestHandler<GetTicketLogQuery, TicketLogsDto>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IRegionalService _regionalService;
        private readonly ITerminal _terminal;

        public GetTicketLogQueryHandler(ITicketRepository ticketRepository, IRegionalService regionalService, ITerminal terminal)
        {
            _ticketRepository = ticketRepository;
            _regionalService = regionalService;
            _terminal = terminal;
        }

        public async Task<TicketLogsDto> Handle(GetTicketLogQuery request, CancellationToken cancellationToken)
        {
            var dto = new TicketLogsDto();

            var timeZone = _terminal.LocalTimeZone;

            var result =
                await _ticketRepository.GetTicketsAsync
                    (barcode: request.Barcode, number: request.Number,
                     printingCompletedAtFrom: request.PrintingCompletedAtFrom, printingCompletedAtTo: request.PrintingCompletedAtTo,
                     printingRequestedAtFrom: request.PrintingRequestedAtFrom, printingRequestedAtTo: request.PrintingRequestedAtTo,
                     daysValidFrom: request.DaysValidFrom, daysValidTo: request.DaysValidTo,
                     isLocal: request.IsLocal, isPrinted: request.IsPrinted,
                     isRedeemed: request.IsRedeemed, isStacked: request.IsStacked,
                     isExpired: request.IsExpired, typeId: request.TypeId,
                     amountFrom: request.AmountFrom, amountTo: request.AmountTo,
                     page: request.Page, pageSize: request.PageSize);

            if (result != null)
            {
                dto.TicketTypes = Enumeration.GetAll<TicketType>().ToList();

                dto.Page = result.Page;
                dto.PageSize = result.PageSize;
                dto.TotalCount = result.TotalCount;
                dto.Tickets = result.Data.Select(x => new TicketDto()
                {
                    Id = x.Id,
                    Barcode = x.Barcode,
                    Number = x.Number,
                    Amount = x.Amount,
                    Type = x.Type,
                    Created = x.Created.HasValue ? _regionalService.ConvertToUserTimeZone(x.Created.Value, timeZone) : null,
                    Currency = x.Currency
                })
                .ToList();
            }

            return await Task.FromResult(dto);
        }
    }
}
