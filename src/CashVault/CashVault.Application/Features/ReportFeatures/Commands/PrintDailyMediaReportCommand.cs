using CashVault.Application.Features.DeviceFeatures.Queries;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.ReportFeatures.Commands
{
    public record PrintDailyMediaReportCommand : IRequest<Unit>
    {
        public DailyMediaReportDto DailyMediaReportDto { get; set; } = new DailyMediaReportDto();
        public DateTime Date { get; set; }
    }

    internal sealed class PrintDailyMediaReportCommandHandler : IRequestHandler<PrintDailyMediaReportCommand, Unit>
    {
        private readonly ITerminal _terminal;
        private readonly IRegionalService _regionalService;

        public PrintDailyMediaReportCommandHandler(ITerminal terminal, IRegionalService regionalService)
        {
            _terminal = terminal;
            _regionalService = regionalService;
        }

        public Task<Unit> Handle(PrintDailyMediaReportCommand request, CancellationToken cancellationToken)
        {
            if (_terminal.TITOPrinter is null || _terminal.TITOPrinter.IsActive == false)
            {
                throw new ArgumentException("Ticket printer not initialized!");
            }

            List<string> lines = new();
            var dailyMedia = request.DailyMediaReportDto;

            var toDate = _regionalService.ConvertToUserTimeZone(request.Date, _terminal.LocalTimeZone).ToString();

            lines.Add($"---------------------------------------------");
            lines.Add("Daily Media Report");
            lines.Add($"Date: {toDate}");
            lines.Add(string.Empty);

            //bills
            lines.Add($"Bills");
            lines.Add($"Accepted count: {dailyMedia.BillsAcceptedCount}, Accepted value: {dailyMedia.BillsAcceptedValue}");
            lines.Add($"Dispensed count: {dailyMedia.BillsDispensedCount}, Dispensed value: {dailyMedia.BillsDispensedValue}");
            lines.Add($"Rejected by acceptor count: {dailyMedia.BillsRejectedByAcceptorCount}");
            lines.Add($"Rejected by dispenser count: {dailyMedia.BillsRejectedByDispenserCount}");

            //tickets
            lines.Add(string.Empty);
            lines.Add($"Tickets");
            lines.Add($"Accepted count: {dailyMedia.TicketsAcceptedCount}, Accepted value: {dailyMedia.TicketsAcceptedValue}");
            lines.Add($"Printed count: {dailyMedia.TicketsPrintedCount}, Printed value: {dailyMedia.TicketsPrintedValue}");
            lines.Add($"Rejected by acceptor count {dailyMedia.TicketsRejectedByAcceptorCount}");
            lines.Add($"---------------------------------------------");

            _terminal.TITOPrinter.PrintTextAsync([.. lines]);

            return Task.FromResult(Unit.Value);
        }
    }

    public class PrintDailyMediaReportCommandValidator : AbstractValidator<PrintDailyMediaReportCommand>
    {
        public PrintDailyMediaReportCommandValidator()
        {
            RuleFor(x => x.DailyMediaReportDto).NotNull().WithMessage("Daily media report is required");
        }
    }
}
