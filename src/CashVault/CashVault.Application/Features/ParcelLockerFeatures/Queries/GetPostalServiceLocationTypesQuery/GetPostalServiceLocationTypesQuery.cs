using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using CashVault.Domain.Common;
using MediatR;

namespace CashVault.Application.Features.ParcelLockerFeatures.Queries;

public class GetPostalServiceLocationTypesQuery
    : IRequest<List<PostalServiceLocationTypeDto>>
{ }

internal sealed class GetPostalServiceLocationTypesQueryHandler : IRequestHandler<GetPostalServiceLocationTypesQuery, List<PostalServiceLocationTypeDto>>
{
    public Task<List<PostalServiceLocationTypeDto>> Handle(GetPostalServiceLocationTypesQuery request, CancellationToken cancellationToken)
    {
        List<PostalServiceLocationTypeDto> result = Enumeration.GetAll<PostalServiceLocationType>()
            .Select(x => new PostalServiceLocationTypeDto
            {
                Code = x.Code,
                Name = x.Code
            }).ToList();

        return Task.FromResult(result);
    }
}
