using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.ParcelLockerFeatures.Queries;

public record GetParcelLockerAvailableSizesQuery : IRequest<List<ParcelLockerSizeDto>>
{
    public string? PostalService { get; set; } = string.Empty;
}

public class GetParcelLockerAvailableSizesQueryValidator : AbstractValidator<GetParcelLockerAvailableSizesQuery>
{
    public GetParcelLockerAvailableSizesQueryValidator()
    {
        RuleFor(x => x.PostalService).NotEmpty().WithMessage("Postal service is required.");
    }
}

internal sealed class GetParcelLockerAvailableSizesQueryHandler
    : IRequestHandler<GetParcelLockerAvailableSizesQuery, List<ParcelLockerSizeDto>>
{
    private readonly ITerminalRepository _db;

    public GetParcelLockerAvailableSizesQueryHandler(
        ITerminalRepository db)
    {
        _db = db;
    }

    public async Task<List<ParcelLockerSizeDto>> Handle(GetParcelLockerAvailableSizesQuery request, CancellationToken cancellationToken)
    {
        IParcelLockerConfiguration? configuration = await _db.GetParcelLockerConfigurationAsync();

        if (configuration is null || configuration.ParcelLockers.Count == 0)
            return [];

        IEnumerable<ParcelLocker> availableLockers = configuration.ParcelLockers
            .Where(x => (string.IsNullOrWhiteSpace(x.PostalService) || x.PostalService.Equals(request.PostalService)) &&
                        x.IsActive && x.IsEmpty);

        return availableLockers.Select(x => x.Size)
            .DistinctBy(x => x.Code)
            .Select(x => new ParcelLockerSizeDto()
            {
                Code = x.Code,
                Name = x.Name,
                Description = x.DimensionDisplayName
            }).ToList();
    }
}
