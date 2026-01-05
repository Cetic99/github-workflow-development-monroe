using CashVault.Application.Features.DeviceFeatures.Queries;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.ReportFeatures.Commands
{
    public record PrintEndOfShiftReportCommand : IRequest<Unit>
    {
        public EndOfShiftReportDto EndOfShiftReportDto { get; set; } = new EndOfShiftReportDto();
    }

    internal sealed class PrintEndOfShiftReportCommandHandler : IRequestHandler<PrintEndOfShiftReportCommand, Unit>
    {
        private readonly ITerminal _terminal;
        private readonly IRegionalService _regionalService;

        public PrintEndOfShiftReportCommandHandler(ITerminal terminal, IRegionalService regionalService)
        {
            _terminal = terminal;
            _regionalService = regionalService;
        }

        public Task<Unit> Handle(PrintEndOfShiftReportCommand request, CancellationToken cancellationToken)
        {
            if (_terminal.TITOPrinter is null || _terminal.TITOPrinter.IsActive == false)
            {
                throw new ArgumentException("Ticket printer not initialized!");
            }

            List<string> lines = new();
            var endOfShift = request.EndOfShiftReportDto;

            var fromDate = endOfShift.FromDate.HasValue ? _regionalService.ConvertToUserTimeZone(endOfShift.FromDate.Value, _terminal.LocalTimeZone).ToString() : "";
            var toDate = _regionalService.ConvertToUserTimeZone(endOfShift.ToDate, _terminal.LocalTimeZone).ToString();

            lines.Add($"---------------------------------------------");
            lines.Add("End of shift");
            lines.Add($"From date: {fromDate}");
            lines.Add($"To date: {toDate}");
            lines.Add(string.Empty);

            //bills
            lines.Add($"Bills");
            lines.Add($"Accepted count: {endOfShift.BillsAcceptedCount}, Accepted value: {endOfShift.BillsAcceptedValue}");
            lines.Add($"Dispensed count: {endOfShift.BillsDispensedCount}, Dispensed value: {endOfShift.BillsDispensedValue}");
            lines.Add($"Rejected by acceptor count: {endOfShift.BillsRejectedByAcceptorCount}");
            lines.Add($"Rejected by dispenser count: {endOfShift.BillsRejectedByDispenserCount}");

            //tickets
            lines.Add(string.Empty);
            lines.Add($"Tickets");
            lines.Add($"Accepted count: {endOfShift.TicketsAcceptedCount}, Accepted value: {endOfShift.TicketsAcceptedValue}");
            lines.Add($"Printed count: {endOfShift.TicketsPrintedCount}, Printed value: {endOfShift.TicketsPrintedValue}");
            lines.Add($"Rejected by acceptor count {endOfShift.TicketsRejectedByAcceptorCount}");
            lines.Add($"---------------------------------------------");

            _terminal.TITOPrinter.PrintTextAsync([.. lines]);

            return Task.FromResult(Unit.Value);
        }
    }

    public class PrintEndOfShiftReportCommandValidator : AbstractValidator<PrintEndOfShiftReportCommand>
    {
        public PrintEndOfShiftReportCommandValidator()
        {
            RuleFor(x => x.EndOfShiftReportDto).NotNull().WithMessage("End of shift report is required.");
        }
    }
}
