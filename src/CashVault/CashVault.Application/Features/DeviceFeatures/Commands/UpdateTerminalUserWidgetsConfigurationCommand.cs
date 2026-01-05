using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands
{
    public class UserWidgetDto
    {
        public Guid Uuid { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Size { get; set; } = UserWidgetSize.DefaultWidgetSize.Code;
        public int DisplaySequence { get; set; } = 9999;
        public bool Enabled { get; set; } = true;
    }

    public class UpdateTerminalUserWidgetsConfigurationCommand : IRequest<Unit>
    {
        public List<UserWidgetDto>? Widgets { get; set; }
    }

    public class UpdateTerminalUserWidgetsConfigurationCommandValidator
        : AbstractValidator<UpdateTerminalUserWidgetsConfigurationCommand>
    {
        public UpdateTerminalUserWidgetsConfigurationCommandValidator()
        {
            RuleFor(c => c.Widgets)
                .NotNull()
                .Must(widgets => widgets != null && widgets.Count > 0 && widgets.Any(w => w.Enabled))
                .WithMessage("At least one enabled widget must be provided.");

            RuleForEach(c => c.Widgets).Must(w => Enumeration.Contains<UserWidgetSize>(w.Size))
                .WithMessage("Widget size is not valid.");
        }
    }

    public class UpdateTerminalUserWidgetsConfigurationCommandHandler(IUnitOfWork unitOfWork, ITerminal terminal)
        : IRequestHandler<UpdateTerminalUserWidgetsConfigurationCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateTerminalUserWidgetsConfigurationCommand request, CancellationToken cancellationToken)
        {
            UserWidgetsConfiguration config = await unitOfWork.TerminalRepository.GetUserWidgetsConfigurationAsync();
            AvailableUserWidgetsConfiguration availableConfig = await unitOfWork.TerminalRepository.GetAvailableUserWidgetsConfigurationAsync();

            config ??= new();

            IEnumerable<string> enabledWidgetCodes = request.Widgets?.Where(x => x.Enabled)?.Select(x => x.Code)
                ?? throw new ArgumentNullException("Widgets must be defined");

            if (availableConfig.AvailableWidgets.All(w => !enabledWidgetCodes.Contains(w.Code)))
                throw new ArgumentException(nameof(UserWidget));

            config.SetWidgets(request.Widgets?.Select(w => new UserWidget()
            {
                Uuid = w.Uuid,
                Code = w.Code,
                Size = Enumeration.GetByCode<UserWidgetSize>(w.Size) ?? UserWidgetSize.DefaultWidgetSize,
                DisplaySequence = w.DisplaySequence,
                Enabled = w.Enabled
            })?.ToList());

            unitOfWork.TerminalRepository.UpdateUserWidgetsConfiguration(config);
            terminal.UpdateUserWidgetsConfiguration(config);

            await unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
