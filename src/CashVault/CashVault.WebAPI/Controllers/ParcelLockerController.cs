using CashVault.Application.Features.ParcelLockerFeatures.Commands;
using CashVault.Application.Features.ParcelLockerFeatures.Models;
using CashVault.Application.Features.ParcelLockerFeatures.Queries;
using CashVault.Application.Features.ParcelLockerFeatures.QueryModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashVault.WebAPI.Controllers;

[Route("api")]
[AllowAnonymous]
public class ParcelLockerController : BaseController
{
    [HttpPost("parcel-locker/pickup")]
    public async Task<ActionResult<PickupPaymentDetails>> RequestPickup(
        [FromBody] RequestShipmentPickupCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpPost("parcel-locker/verify-courier")]
    public async Task<ActionResult<CourierModel>> VerifyCourier(
        [FromBody] VerifyCourierBarcodeCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpPut("parcel-locker/assign-shipment")]
    public async Task<ActionResult<AssignShipmentToLockerResponse>> AssignShipment(
        [FromBody] AssignShipmentToLockerCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpPost("parcel-locker/shipment")]
    public async Task<IActionResult> CreateShipmentV2(
        [FromBody] CreateShipmentCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpPut("parcel-locker/open")]
    public async Task<ActionResult<OpenLockerForShipmentResponse>> OpenParcelLocker(
        [FromBody] OpenLockerForShipmentCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpPut("parcel-locker/shipment/{trackingNumber}/change-locker")]
    public async Task<IActionResult> ChangeShipmentParcelLocker(
        [FromRoute] string trackingNumber,
        [FromBody] ChangeShipmentLockerSizeCommand command)
    {
        return Ok(await Mediator.Send(command));
    }


    [HttpGet("parcel-locker/postal-services")]
    public async Task<ActionResult<List<PostalServiceDto>>> GetAvailablePostalServices()
    {
        return await Mediator.Send(new GetPostalServicesQuery());
    }

    [HttpGet("parcel-locker/payment-options")]
    public async Task<ActionResult<List<PaymentOptionDto>>> GetSupportedPaymentOptions()
    {
        return await Mediator.Send(new GetPaymentOptionsQuery());
    }

    [HttpGet("parcel-locker/locker-sizes")]
    public async Task<ActionResult<List<ParcelLockerSizeDto>>> GetAvailableParcelLockerSizes(
        [FromQuery] string? postalService)
    {
        return await Mediator.Send(new GetParcelLockerAvailableSizesQuery()
        {
            PostalService = postalService
        });
    }

    [HttpGet("parcel-locker/delivery-options")]
    public async Task<ActionResult<List<DeliveryOptionDto>>> GetAvailableDeliveryOptions(
        [FromQuery] string? postalService,
        [FromQuery] string? parcelLockerSize)
    {
        return await Mediator.Send(new GetDeliveryOptionsQuery()
        {
            PostalService = postalService,
            ParcelLockerSize = parcelLockerSize
        });
    }

    [HttpGet("parcel-locker/location-types")]
    public async Task<ActionResult<List<PostalServiceLocationTypeDto>>> GetAvailableLocationTypes()
    {
        return await Mediator.Send(new GetPostalServiceLocationTypesQuery());
    }

    [HttpGet("parcel-locker/search/postal-services")]
    public async Task<ActionResult<List<PostalServiceLocationDto>>> SearchPostalServicesLocations(
        [FromQuery] string? query = null,
        [FromQuery] List<string>? postalService = null,
        [FromQuery] List<string>? locationTypes = null,
        [FromQuery] bool? forSend = null,
        [FromQuery] bool? forReceive = null)
    {
        return await Mediator.Send(new GetPostalServicesLocationsQuery()
        {
            Query = query,
            PostalServices = postalService,
            LocationTypes = locationTypes,
            ForSending = forSend,
            ForReceiving = forReceive
        });
    }

    [HttpGet("parcel-locker/courier/{courierId}/shipments-in-lockers")]
    public async Task<ActionResult<List<ShipmentDto>>> GetShipmentsInLockersForCourier(
        [FromRoute] string courierId,
        [FromQuery] string postalService)
    {
        return Ok(await Mediator.Send(new GetShipmentsInLockersQuery()
        {
            CourierId = courierId,
            PostalService = postalService
        }));
    }

    [HttpGet("parcel-locker/courier/{courierId}/pending-shipments")]
    public async Task<ActionResult<List<ShipmentDto>>> GetPendingShipmentsForCourier(
        [FromRoute] string courierId,
        [FromQuery] string postalService)
    {
        return Ok(await Mediator.Send(new GetPendingShipmentsQuery()
        {
            CourierId = courierId,
            PostalService = postalService
        }));
    }


    [HttpPut("parcel-locker/courier/{courierId}/pickup")]
    public async Task<ActionResult<RequestShipmentPickupByCourierResponse>> PickupShipmentByCourier(
        [FromBody] RequestShipmentPickupByCourierCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
}
