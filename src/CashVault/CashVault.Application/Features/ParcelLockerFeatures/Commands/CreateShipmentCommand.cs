using CashVault.Application.Features.ParcelLockerFeatures.Models;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using CashVault.Domain.Aggregates.TransactionAggregate;
using CashVault.Domain.Common;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.ParcelLockerFeatures.Commands;

public class UserInformationsDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
}

public class LocationDto
{
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
}

public record CreateShipmentCommand : IRequest<ShipmentDetailsModel>
{
    public string PostalService { get; set; } = string.Empty;
    public string DeliveryOption { get; set; } = string.Empty;
    public string ParcelLockerSize { get; set; } = string.Empty;
    public LocationDto? Location { get; set; }
    public UserInformationsDto? Recipient { get; set; }
    public UserInformationsDto? Sender { get; set; }
}

public class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
{
    public CreateShipmentCommandValidator()
    {
        RuleFor(x => x.PostalService).NotEmpty().WithMessage("Postal service is required.");
        RuleFor(x => x.ParcelLockerSize).NotEmpty().Must(Enumeration.Contains<ParcelLockerSize>).WithMessage("Parcel locker is required.");
        RuleFor(x => x.DeliveryOption).NotEmpty().Must(Enumeration.Contains<DeliveryOption>).WithMessage("Delivery option is required.");
        RuleFor(x => x.Location).NotNull().Must(x => !string.IsNullOrEmpty(x?.City) &&
                                                      !string.IsNullOrEmpty(x?.Address) &&
                                                       !string.IsNullOrEmpty(x?.PostalCode))
            .WithMessage("Location is required.");
        RuleFor(x => x.Recipient).NotNull().Must(x => !string.IsNullOrEmpty(x?.PhoneNumber) &&
                                                       !string.IsNullOrEmpty(x?.FirstName) &&
                                                        !string.IsNullOrEmpty(x?.LastName))
            .WithMessage("Recepient informations are required.");
        RuleFor(x => x.Sender).NotNull().Must(x => !string.IsNullOrEmpty(x?.PhoneNumber))
            .WithMessage("Sender informations are required.");
    }
}

internal sealed class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, ShipmentDetailsModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPostalServiceProvider _postalServiceProvider;

    public CreateShipmentCommandHandler(
        IUnitOfWork unitOfWork,
        IPostalServiceProvider postalServiceProvider)
    {
        _unitOfWork = unitOfWork;
        _postalServiceProvider = postalServiceProvider;
    }

    public async Task<ShipmentDetailsModel> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            PostalServicesConfiguration postalServicesConfiguration = await _unitOfWork.TerminalRepository.GetPostalServicesConfigurationAsync()
                ?? throw new InvalidOperationException("Postal services configuration not found.");

            PostalService postalService = postalServicesConfiguration.GetPostalService(request.PostalService)
                ?? throw new InvalidOperationException("Postal service not found.");

            IParcelLockerConfiguration parcelLockerConfiguration = await _unitOfWork.TerminalRepository
                .GetParcelLockerConfigurationAsync()
                ?? throw new InvalidOperationException("Parcel locker configuration not found.");

            ParcelLocker? parcelLocker = parcelLockerConfiguration.GetAvailableLocker(postalService.Code, request.ParcelLockerSize)
                ?? throw new InvalidOperationException("Parcel locker not found.");

            CreateShipmentModel createShipmentModel = new()
            {
                RecipientFirstName = request.Recipient.FirstName,
                RecipientLastName = request.Recipient.LastName,
                RecipientPhoneNumber = request.Recipient.PhoneNumber,

                Address = request.Location.Address,
                PostalCode = request.Location.PostalCode,
                City = request.Location.City,
                Country = request.Location.Country,

                SenderPhoneNumber = request.Sender.PhoneNumber,

                PostalService = request.PostalService
            };

            // todo
            ShipmentDetailsModel? shipmentDetails = await _postalServiceProvider.CreateShipmentAsync(createShipmentModel)
                ?? throw new InvalidOperationException("Failed to create shipment.");

            // here should be different amounts based on locker size?
            PostalServiceDeliveryOption? deliveryOption = postalService.GetDeliveryOption(
                deliveryOption: Enumeration.GetByCode<DeliveryOption>(request.DeliveryOption),
                lockerSize: Enumeration.GetByCode<ParcelLockerSize>(request.ParcelLockerSize));

            ParcelLockerShipment shipment = new(
                barcode: shipmentDetails.Barcode,
                registrationNumber: shipmentDetails.RegistrationNumber,
                lockerAccessCode: shipmentDetails.LockerAccessCode,
                postalService: request.PostalService,
                parcelLocker: parcelLocker,
                paymentRequired: deliveryOption?.RequiresPayment ?? false,
                amount: deliveryOption?.Amount,
                currency: deliveryOption?.Currency,
                expirationDate: DateTime.MaxValue,
                reciever: new ShipmentParty()
                {
                    FirstName = request.Recipient.FirstName,
                    LastName = request.Recipient.LastName,
                    PhoneNumber = request.Recipient.PhoneNumber,
                },
                address: new Address(
                    country: request.Location.Country,
                    city: request.Location.City,
                    postalCode: request.Location.PostalCode,
                    streetName: request.Location.Address,
                    streetNumber: string.Empty
                ),
                addressLocationType: Enumeration.GetByCode<PostalServiceLocationType>(request.Location.LocationType),
                sender: new ShipmentParty()
                {
                    FirstName = request.Sender.FirstName,
                    LastName = request.Sender.LastName,
                    PhoneNumber = request.Sender.PhoneNumber
                });

            //parcelLocker.AssignShipment(shipment);
            //_unitOfWork.TerminalRepository.UpdateParcelLockerConfiguration(parcelLockerConfiguration);

            _unitOfWork.ParcelLockerRepository.SaveShipment(shipment);

            if (shipment.PaymentRequired) // create transaction
            {
                ParcelLockerTransaction transaction = new(
                    shipment,
                    parcelLocker,
                    shipment.Amount ?? 0,
                    currency: shipment.Currency);

                await _unitOfWork.TransactionRepository.SaveTransactionAsync(transaction);
            }

            await _unitOfWork.SaveChangesAsync();


            return shipmentDetails;
        }
        catch (Exception e)
        {
            Console.WriteLine("A");
        }

        return null;
    }
}