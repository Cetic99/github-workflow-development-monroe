using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using CashVault.Domain.Common;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.ParcelLockerFeatures.Queries;

public class GetDeliveryOptionsQuery : IRequest<List<DeliveryOptionDto>>
{
    public string? PostalService { get; set; }
    public string? ParcelLockerSize { get; set; }
}

public class GetDeliveryOptionsQueryValidator : AbstractValidator<GetDeliveryOptionsQuery>
{
    public GetDeliveryOptionsQueryValidator()
    {
        RuleFor(x => x.PostalService).NotEmpty().WithMessage("Postal service is required.");
        RuleFor(x => x.ParcelLockerSize).NotEmpty().Must(Enumeration.Contains<ParcelLockerSize>).WithMessage("Parcel locker size is required.");
    }
}

internal sealed class GetDeliveryOptionsQueryHandler(ITerminalRepository db)
    : IRequestHandler<GetDeliveryOptionsQuery, List<DeliveryOptionDto>>
{
    public async Task<List<DeliveryOptionDto>> Handle(GetDeliveryOptionsQuery request, CancellationToken cancellationToken)
    {
        PostalServicesConfiguration? configuration = await db.GetPostalServicesConfigurationAsync();

        if (configuration is null || configuration.PostalServices.Count < 1)
            return [];

        PostalService? postalService = configuration.PostalServices.FirstOrDefault(x => x.Code.Equals(request.PostalService));

        if (postalService is null || postalService.DeliveryOptions.Count < 1)
            return [];

        return postalService.DeliveryOptions.Where(x => x.ParcelLockerSize.Code.Equals(request.ParcelLockerSize))
            .Select(x => new DeliveryOptionDto()
            {
                Code = x.DeliveryOption.Code,
                PaymentRequired = x.RequiresPayment,
                Amount = x.Amount,
                Currency = x.Currency,
            }).ToList();
    }
}
