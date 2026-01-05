using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CashVault.Application.Features.ParcelLockerFeatures.Commands;

public class RequestShipmentPickupByCourierCommand : IRequest<RequestShipmentPickupByCourierResponse>
{
    public string PostalService { get; init; } = string.Empty;
    public string? Barcode { get; init; } = string.Empty;
    public long AccessCode { get; init; }
    public string CourierId { get; init; } = string.Empty;
}

public class RequestShipmentPickupByCourierResponse
{
    public bool IsOpen { get; set; }
    public int? ParcelLockerId { get; set; } // or some other prop
}

public class RequestShipmentPickupByCourierCommandValidator : AbstractValidator<RequestShipmentPickupByCourierCommand>
{
    public RequestShipmentPickupByCourierCommandValidator()
    {
        RuleFor(x => x.PostalService).NotEmpty().WithMessage("Postal service is required.");
        RuleFor(x => x).Must(x => !string.IsNullOrWhiteSpace(x.Barcode) || x.AccessCode > 0)
            .WithMessage("Shipment barcode is required.");
        RuleFor(x => x.CourierId).NotEmpty().WithMessage("Courier is required.");
    }
}

internal sealed class RequestShipmentPickupByCourierCommandHandler : IRequestHandler<RequestShipmentPickupByCourierCommand, RequestShipmentPickupByCourierResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITerminal _terminal;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RequestShipmentPickupByCourierCommandHandler(
        IUnitOfWork unitOfWork,
        ITerminal terminal,
        IServiceScopeFactory serviceScopeFactory)
    {
        _unitOfWork = unitOfWork;
        _terminal = terminal;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<RequestShipmentPickupByCourierResponse> Handle(RequestShipmentPickupByCourierCommand request,
                                                                     CancellationToken cancellationToken)
    {
        ParcelLockerShipment shipment = await _unitOfWork.ParcelLockerRepository
            .GetShipment(barcode: request.Barcode ?? string.Empty, accessCode: request.AccessCode, postalService: request.PostalService)
            ?? throw new InvalidOperationException("Shipment not found.");

        IParcelLockerConfiguration parcelLockerConfiguration = await _unitOfWork.TerminalRepository
            .GetParcelLockerConfigurationAsync()
            ?? throw new InvalidOperationException("Parcel locker configuration not found.");

        ParcelLocker parcelLocker = parcelLockerConfiguration.GetLocker(x => shipment.Barcode.Equals(x.Shipment))
             ?? throw new InvalidOperationException("Parcel locker not found.");

        bool canBePickedUp = shipment.CheckIfCanBePickedUpByCourier();

        if (!canBePickedUp)
            throw new InvalidOperationException("Shipment cannot be picked up.");

        bool lockerOpened = true;
        int lockerId = parcelLocker.Id;

        if (lockerOpened)
        {
            shipment.Accept(request.CourierId);
            await _unitOfWork.SaveChangesAsync();

            CancellationTokenSource cancellationLockerCloseTokenSource = new(TimeSpan.FromMinutes(10));
            _ = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                IParcelLockerConfiguration? config = await unitOfWork.TerminalRepository.GetParcelLockerConfigurationAsync();
                ParcelLocker? locker = config?.GetLocker(lockerId);

                if (config is null || locker is null) // log error
                    return;

                try
                {
                    bool lockerClosed = false;

                    while (!lockerClosed)
                    {
                        await Task.Delay(200);

                        if (cancellationLockerCloseTokenSource.Token.IsCancellationRequested)
                            cancellationLockerCloseTokenSource.Token.ThrowIfCancellationRequested();

                        lockerClosed = true;
                    }

                    locker.Empty();
                }
                catch (OperationCanceledException e)
                {
                    locker.SetError($"Parcel locker is not closed: {e.Message}");
                    // should we cancell shipment here?
                }
                catch (Exception e)
                {
                    locker.SetError($"Error while closing parcel locker: {e.Message}");
                }
                finally
                {
                    unitOfWork.TerminalRepository.UpdateParcelLockerConfiguration(config);

                    await unitOfWork.SaveChangesAsync();
                    await notificationService.ParcelLockerClosed(locker.Id);
                }
            }, cancellationLockerCloseTokenSource.Token);
        }

        // payment ?

        return new RequestShipmentPickupByCourierResponse()
        {
            IsOpen = lockerOpened,
            ParcelLockerId = lockerOpened ? lockerId : null
        };
    }
}
