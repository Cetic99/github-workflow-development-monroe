using CashVault.Application.Features.ParcelLockerFeatures.Models;
using CashVault.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.ParcelLockerFeatures.Commands;

public class VerifyCourierBarcodeCommand : IRequest<CourierModel>
{
    public string PostalService { get; init; } = string.Empty;
    public string Barcode { get; init; } = string.Empty;
}

public class ProcessCourierBarcodeCommandValidator : AbstractValidator<VerifyCourierBarcodeCommand>
{
    public ProcessCourierBarcodeCommandValidator()
    {
        RuleFor(x => x.PostalService).NotEmpty().WithMessage("Postal service is required.");
        RuleFor(x => x.Barcode).NotEmpty().WithMessage("Courier barcode is required.")
            .Matches(@"^\d{8,20}$").WithMessage("Barcode must be between 8 and 20 digits.");
    }
}

internal sealed class ProcessCourierBarcodeCommandHandler : IRequestHandler<VerifyCourierBarcodeCommand, CourierModel>
{
    private readonly IPostalServiceProvider _postalServiceProvider;

    public ProcessCourierBarcodeCommandHandler(IPostalServiceProvider postalServiceProvider)
    {
        _postalServiceProvider = postalServiceProvider;
    }

    public async Task<CourierModel> Handle(VerifyCourierBarcodeCommand request, CancellationToken cancellationToken)
    {
        CourierModel courier = await _postalServiceProvider.ProcessCourierBarcode(
            request.PostalService, request.Barcode)
            ?? throw new InvalidOperationException("Courier not found.");

        return courier;
    }
}