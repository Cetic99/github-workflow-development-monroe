using CashVault.Application.Interfaces.Persistence;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class GetTeminalUserWidgetsQuery
        : IRequest<List<TerminalUserWidgetDto>>
    { }

    internal sealed class GetTeminalUserWidgetsQueryHandler : IRequestHandler<GetTeminalUserWidgetsQuery, List<TerminalUserWidgetDto>>
    {
        private readonly ITerminalRepository _db;

        public GetTeminalUserWidgetsQueryHandler(ITerminalRepository db)
        {
            _db = db;
        }

        public async Task<List<TerminalUserWidgetDto>> Handle(GetTeminalUserWidgetsQuery request, CancellationToken cancellationToken)
        {
            var userWidgets = await _db.GetUserWidgetsConfigurationAsync();

            if (userWidgets?.Widgets is null || userWidgets.Widgets.Count == 0)
                return [];

            var availableUserWidgets = await _db.GetAvailableUserWidgetsConfigurationAsync();

            if (availableUserWidgets?.AvailableWidgets is null || availableUserWidgets.AvailableWidgets.Count == 0)
                return [];

            var availableCodes = availableUserWidgets.AvailableWidgets
                .Select(aw => aw.Code)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return userWidgets.Widgets
                .Where(w => availableCodes.Contains(w.Code) && w.Enabled)
                .OrderBy(w => w.DisplaySequence)
                .Select(w => new TerminalUserWidgetDto()
                {
                    Uuid = w.Uuid,
                    Code = w.Code,
                    DisplaySequence = w.DisplaySequence,
                    Size = w.Size.Code
                }).ToList();
        }
    }
}
