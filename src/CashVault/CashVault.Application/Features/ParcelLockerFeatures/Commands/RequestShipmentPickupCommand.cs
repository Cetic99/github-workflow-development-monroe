using CashVault.Application.Features.ParcelLockerFeatures.Models;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using CashVault.Domain.Aggregates.TransactionAggregate;
using CashVault.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.ParcelLockerFeatures.Commands;

public class RequestShipmentPickupCommand : IRequest<PickupPaymentDetails>
{
    public string PostalService { get; init; } = string.Empty;
    public string? Barcode { get; init; } = string.Empty;
    public long AccessCode { get; init; }
}

public class RequestShipmentPickupCommandValidator : AbstractValidator<RequestShipmentPickupCommand>
{
    public RequestShipmentPickupCommandValidator()
    {
        RuleFor(x => x.PostalService).NotEmpty().WithMessage("Postal service is required.");
        RuleFor(x => x).Must(x => !string.IsNullOrWhiteSpace(x.Barcode) || x.AccessCode > 0)
            .WithMessage("Shipment barcode is required.");
    }
}

internal sealed class RequestShipmentPickupCommandHandler : IRequestHandler<RequestShipmentPickupCommand, PickupPaymentDetails>
{
    private readonly IUnitOfWork _unitOfWork;

    public RequestShipmentPickupCommandHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PickupPaymentDetails> Handle(RequestShipmentPickupCommand request, CancellationToken cancellationToken)
    {
        ParcelLockerShipment shipment = await _unitOfWork.ParcelLockerRepository
            .GetShipment(barcode: request.Barcode ?? string.Empty, accessCode: request.AccessCode, postalService: request.PostalService)
            ?? throw new InvalidOperationException("Shipment not found.");

        bool canBePickedUp = shipment.CheckIfCanBePickedUp();

        if (!canBePickedUp)
        {
            bool expired = shipment.Expire();

            await _unitOfWork.SaveChangesAsync();

            if (expired) throw new InvalidOperationException("Shipment expired.");
            else throw new InvalidOperationException("Shipment cannot be picked up.");
        }

        IParcelLockerConfiguration parcelLockerConfiguration = await _unitOfWork.TerminalRepository
            .GetParcelLockerConfigurationAsync()
            ?? throw new InvalidOperationException("Parcel locker configuration not found.");

        ParcelLocker parcelLocker = parcelLockerConfiguration.GetLocker(x => shipment.Barcode.Equals(x.Shipment))
             ?? throw new InvalidOperationException("Parcel locker not found.");

        if (shipment.PaymentRequired) // create transaction
        {
            ParcelLockerTransaction transaction = new(
                shipment,
                parcelLocker,
                shipment.Amount ?? 0,
                currency: shipment.Currency);

            await _unitOfWork.TransactionRepository.SaveTransactionAsync(transaction);
        }

        return new PickupPaymentDetails()
        {
            PaymentRequired = shipment.PaymentRequired,
            Amount = shipment.Amount,
            Currency = shipment.Currency ?? Currency.Default
        };
    }
}
