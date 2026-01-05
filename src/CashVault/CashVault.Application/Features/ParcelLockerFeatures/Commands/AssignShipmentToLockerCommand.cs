using CashVault.Application.Features.ParcelLockerFeatures.Models;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using CashVault.Domain.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CashVault.Application.Features.ParcelLockerFeatures.Commands;

public class AssignShipmentToLockerCommand : IRequest<AssignShipmentToLockerResponse>
{
    public string PostalService { get; set; } = string.Empty;
    public string? Barcode { get; set; } = string.Empty;
    public long AccessCode { get; set; }
    public string? CourierId { get; set; }
}

public class AssignShipmentToLockerResponse
{
    public bool IsOpen { get; set; }
    public int? ParcelLockerId { get; set; } // or some other prop
}

public class AssignShipmentToLockerCommandValidator : AbstractValidator<AssignShipmentToLockerCommand>
{
    public AssignShipmentToLockerCommandValidator()
    {
        RuleFor(x => x.PostalService).NotEmpty().WithMessage("Postal service is required.");
        RuleFor(x => x).Must(x => !string.IsNullOrWhiteSpace(x.Barcode) || x.AccessCode > 0)
            .WithMessage("Shipment barcode is required.");
    }
}

internal sealed class AssignShipmentToLockerCommandHandler : IRequestHandler<AssignShipmentToLockerCommand, AssignShipmentToLockerResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPostalServiceProvider _postalServiceProvider;
    private readonly ITerminal _terminal;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AssignShipmentToLockerCommandHandler(
        IPostalServiceProvider postalServiceProvider,
        IUnitOfWork unitOfWork,
        ITerminal terminal,
        IServiceScopeFactory serviceScopeFactory)
    {
        _unitOfWork = unitOfWork;
        _postalServiceProvider = postalServiceProvider;
        _terminal = terminal;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<AssignShipmentToLockerResponse> Handle(AssignShipmentToLockerCommand request, CancellationToken cancellationToken)
    {
        IParcelLockerConfiguration parcelLockerConfiguration = await _unitOfWork.TerminalRepository
            .GetParcelLockerConfigurationAsync() ?? throw new InvalidOperationException("Parcel locker configuration not found.");

        // if retrying to create shipment, get existing shipment
        ParcelLockerShipment? shipment = await _unitOfWork.ParcelLockerRepository
            .GetShipment(barcode: request.Barcode, accessCode: request.AccessCode, postalService: request.PostalService);

        ParcelLocker? availableLocker = null;

        if (shipment is not null)
            availableLocker = parcelLockerConfiguration.GetLocker(shipment.ParcelLockerId)
                ?? throw new InvalidOperationException("Parcel locker not found.");

        bool isNewShipment = shipment is null;

        // shipment is not created
        if (shipment is null)
        {
            ShipmentModel shipmentModel = await _postalServiceProvider.FetchShipment(
                request.PostalService, request.Barcode, request.AccessCode)
                ?? throw new InvalidOperationException("Shipment not found.");

            shipmentModel.Validate();

            availableLocker = parcelLockerConfiguration.GetAvailableLocker(request.PostalService, shipmentModel.ParcelLockerSize)
                ?? throw new InvalidOperationException("No available parcel lockers.");

            shipment = CreateShipment(shipmentModel, availableLocker, request.CourierId);
        }

        //bool? lockerOpened = await _terminal.ParcelLocker?.OpenCabinetAsync(availableLocker!.Id);
        bool lockerOpened = true;
        int lockerId = availableLocker!.Id;

        if (lockerOpened)
        {
            shipment.Recieve();

            if (isNewShipment) _unitOfWork.ParcelLockerRepository.SaveShipment(shipment);
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

                    locker.AssignShipment(shipment);
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

        return new AssignShipmentToLockerResponse()
        {
            IsOpen = lockerOpened,
            ParcelLockerId = lockerOpened ? lockerId : null
        };
    }

    private static ParcelLockerShipment CreateShipment(
        ShipmentModel shipmentModel,
        ParcelLocker parcelLocker,
        string? courierId = null)
    {
        return new ParcelLockerShipment(
            barcode: shipmentModel.Barcode,
            registrationNumber: shipmentModel.RegistrationNumber,
            lockerAccessCode: shipmentModel.LockerAccessCode,
            status: ShipmentStatus.Announced,
            paymentRequired: shipmentModel.Amount.HasValue && shipmentModel.Amount.Value > 0,
            amount: shipmentModel.Amount,
            currency: Currency.Default,
            parcelLocker: parcelLocker,
            postalService: shipmentModel.PostalService,
            expirationDate: shipmentModel.ExpirationDate,
            reciever: new ShipmentParty() { PhoneNumber = shipmentModel.RecieverPhoneNumber },
            courier: courierId
        );
    }
}