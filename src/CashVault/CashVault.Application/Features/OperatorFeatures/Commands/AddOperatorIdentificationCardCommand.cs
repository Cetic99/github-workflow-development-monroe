using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.OperatorAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.OperatorFeatures.Commands;

public class AddOperatorIdentificationCardCommand
    : IRequest<Unit>
{
    public int OperatorId { get; set; }
}

internal sealed class AddOperatorIdentificationCardCommandHandler
    : IRequestHandler<AddOperatorIdentificationCardCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly ITerminal _terminal;
    private readonly ILogger<AddOperatorIdentificationCardCommandHandler> _logger;
    private readonly ISessionService _sessionService;

    public AddOperatorIdentificationCardCommandHandler(
        ITerminal terminal,
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        ILogger<AddOperatorIdentificationCardCommandHandler> logger,
        ISessionService sessionService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _terminal = terminal;
        _logger = logger;
        _sessionService = sessionService;
    }

    public async Task<Unit> Handle(AddOperatorIdentificationCardCommand command, CancellationToken cancallationToken)
    {
        try
        {
            Operator @operator = await _unitOfWork.OperatorRepository.GetOperatorWithIdentificationCards(command.OperatorId)
                ?? throw new ArgumentException(nameof(Operator));

            IdentificationCard identificationCard = new(
                uuid: Guid.NewGuid(),
                validFrom: DateTime.UtcNow,
                validTo: null,
                issuedBy: "Monroe",
                remarks: string.Empty);

            identificationCard.GenerateSerialNumber(@operator.IdentificationCards.Count + 1);
            @operator.AddNewCard(identificationCard);

            bool? cardEnrolled = _terminal.UserCardReader?.EnrollCard(identificationCard);

            if (!cardEnrolled.HasValue || !cardEnrolled.Value)
            {
                await _notificationService.CardeEnrolmentFailed();

                return Unit.Value;
            }

            await _unitOfWork.SaveChangesAsync();
            await _notificationService.CardEnrolled();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            await _notificationService.CardeEnrolmentFailed();
        }

        return Unit.Value;
    }
}
