using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using CashVault.Domain.Common;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.ParcelLockerFeatures.Commands;

public class ChangeShipmentLockerSizeCommand : IRequest<Unit>
{
    public string Barcode { get; set; } = string.Empty;
    public long AccessCode { get; set; }
    public string LockerSize { get; set; } = string.Empty; // new locker size
    public string PostalService { get; set; } = string.Empty;
}

public class ChangeShipmentLockerSizeCommandValidator : AbstractValidator<ChangeShipmentLockerSizeCommand>
{
    public ChangeShipmentLockerSizeCommandValidator()
    {
        RuleFor(x => x).Must(x => !string.IsNullOrWhiteSpace(x.Barcode) || x.AccessCode > 0)
            .WithMessage("Shipment barcode is required.");
        RuleFor(x => x.LockerSize).NotEmpty().Must(Enumeration.Contains<ParcelLockerSize>).WithMessage("Locker size is required.");
        RuleFor(x => x.PostalService).NotEmpty().WithMessage("Postal service is required.");
    }
}

internal sealed class ChangeShipmentLockerSizeCommandHandler : IRequestHandler<ChangeShipmentLockerSizeCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public ChangeShipmentLockerSizeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ChangeShipmentLockerSizeCommand request, CancellationToken cancellationToken)
    {
        ParcelLockerShipment shipment = await _unitOfWork.ParcelLockerRepository.GetShipment(
            barcode: request.Barcode, accessCode: request.AccessCode, postalService: request.PostalService)
            ?? throw new ArgumentNullException(nameof(ParcelLockerShipment));

        int parcelLockerId = shipment.ParcelLockerId;

        IParcelLockerConfiguration? parcelLockerConfiguration = await _unitOfWork.TerminalRepository.GetParcelLockerConfigurationAsync();
        ParcelLocker oldLocker = parcelLockerConfiguration.GetLocker(parcelLockerId) ?? throw new ArgumentNullException(nameof(ParcelLocker));

        // change transaction

        ParcelLocker newLocker = parcelLockerConfiguration.GetAvailableLocker(request.PostalService, request.LockerSize)
                ?? throw new InvalidOperationException("Parcel locker not found.");

        oldLocker.Empty();
        newLocker.AssignShipment(shipment);
        shipment.ChangeParcelLocker(newLocker);

        _unitOfWork.TerminalRepository.UpdateParcelLockerConfiguration(parcelLockerConfiguration);

        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}