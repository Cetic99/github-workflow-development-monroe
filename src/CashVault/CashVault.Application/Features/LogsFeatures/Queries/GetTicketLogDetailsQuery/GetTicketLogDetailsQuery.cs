using CashVault.Application.Interfaces;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class GetTicketLogDetailsQueryValidator : AbstractValidator<GetTicketLogDetailsQuery>
    {
        public GetTicketLogDetailsQueryValidator()
        {
            RuleFor(p => p.Id).NotEmpty();
        }
    }

    public record GetTicketLogDetailsQuery : IRequest<TicketLogDetailsDto>
    {
        public int Id { get; set; }
    }

    internal sealed class GetTicketLogDetailsQueryHandler : IRequestHandler<GetTicketLogDetailsQuery, TicketLogDetailsDto>
    {
        private readonly ITicketRepository _db;
        private readonly IRegionalService _regionalService;
        private readonly ITerminal _terminal;

        public GetTicketLogDetailsQueryHandler(ITicketRepository db, IRegionalService regionalService, ITerminal terminal)
        {
            _db = db;
            _regionalService = regionalService;
            _terminal = terminal;
        }

        public async Task<TicketLogDetailsDto> Handle(GetTicketLogDetailsQuery request, CancellationToken cancellationToken)
        {
            var dto = new TicketLogDetailsDto();
            var ticket = await _db.GetTicketByIdAsync(request.Id);

            var timeZone = _terminal.LocalTimeZone;

            if (ticket != null)
            {
                dto = new TicketLogDetailsDto()
                {
                    Id = ticket.Id,
                    Barcode = ticket.Barcode,
                    Number = ticket.Number,
                    Amount = ticket.Amount,
                    Type = ticket.Type.Code,
                    Created = ticket.Created.HasValue ? _regionalService.ConvertToUserTimeZone(ticket.Created.Value, timeZone) : null,
                    Currency = ticket.Currency,
                    ExpirationDateTime = _regionalService.ConvertToUserTimeZone(ticket.ExpirationDateTime, timeZone),
                    PrintingRequestedAt = _regionalService.ConvertToUserTimeZone(ticket.PrintingRequestedAt, timeZone),
                    PrintingCompletedAt = ticket.PrintingCompletedAt.HasValue ? _regionalService.ConvertToUserTimeZone(ticket.PrintingCompletedAt.Value, timeZone) : null,
                    DaysValid = ticket.DaysValid,
                    IsLocal = ticket.IsLocal,
                    IsPrinted = ticket.IsPrinted,
                    IsStacked = ticket.IsStacked,
                    IsExpired = ticket.IsExpired,
                    IsValid = ticket.IsValid,
                };
            }

            return await Task.FromResult(dto);
        }
    }
}