using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CashVault.Application.Features.ParcelLockerFeatures.Commands;

public record OpenLockerForShipmentCommand : IRequest<OpenLockerForShipmentResponse>
{
    public string? Barcode { get; set; } = string.Empty;
    public long AccessCode { get; set; }
}

public record OpenLockerForShipmentResponse
{
    public bool IsOpen { get; set; }
    public int? ParcelLockerId { get; set; }
}

public class OpenLockerForShipmentCommandValidator : AbstractValidator<OpenLockerForShipmentCommand>
{
    public OpenLockerForShipmentCommandValidator()
    {
        RuleFor(x => x).Must(x => !string.IsNullOrWhiteSpace(x.Barcode) || x.AccessCode > 0)
            .WithMessage("Shipment barcode is required.");
    }
}

internal sealed class OpenLockerForShipmentCommandHandler : IRequestHandler<OpenLockerForShipmentCommand, OpenLockerForShipmentResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITerminal _terminal;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OpenLockerForShipmentCommandHandler(
        IUnitOfWork unitOfWork,
        ITerminal terminal,
        IServiceScopeFactory serviceScopeFactory)
    {
        _unitOfWork = unitOfWork;
        _terminal = terminal;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<OpenLockerForShipmentResponse> Handle(OpenLockerForShipmentCommand request, CancellationToken cancellationToken)
    {
        ParcelLockerShipment shipment = await _unitOfWork.ParcelLockerRepository
            .GetShipment(barcode: request.Barcode, accessCode: request.AccessCode)
            ?? throw new InvalidOperationException("Shipment not found.");

        IParcelLockerConfiguration parcelLockerConfiguration = await _unitOfWork.TerminalRepository
            .GetParcelLockerConfigurationAsync()
            ?? throw new InvalidOperationException("Parcel locker configuration not found.");

        ParcelLocker parcelLocker = parcelLockerConfiguration.GetLocker(shipment.ParcelLockerId)
             ?? throw new InvalidOperationException("Parcel locker not found.");

        // transaction should already be completed because of payment methods -> should we check if transaction is completed ?

        bool lockerOpened = true;
        int lockerId = parcelLocker.Id;

        //bool parcelOpen = _terminal.ParcelLocker?.OpenLocker(shipment.ParcelLocker) ?? false;

        if (lockerOpened)
        {
            if (ShipmentStatus.Draft.Equals(shipment.Status) || ShipmentStatus.Announced.Equals(shipment.Status))
                shipment.Recieve();
            else
                shipment.Accept();
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

        return new OpenLockerForShipmentResponse()
        {
            IsOpen = lockerOpened,
            ParcelLockerId = lockerOpened ? lockerId : null
        };
    }
}