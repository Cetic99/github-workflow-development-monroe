using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.OperatorAggregate;
using MediatR;

namespace CashVault.Application.Features.OperatorFeatures.Queries.GetPermissionsQuery;

public record GetPermissionsQuery : IRequest<PermissionsDto>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
}

internal sealed class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, PermissionsDto>
{
    private readonly IOperatorRepository _db;
    private readonly List<string> _supportedPermissions =
    [
        PermissionEnum.Reports,
        PermissionEnum.Maintenance,
        PermissionEnum.Configuration,
        PermissionEnum.Administration,
        PermissionEnum.MoneyService,
        PermissionEnum.Logs
    ];

    public GetPermissionsQueryHandler(IOperatorRepository db)
    {
        _db = db;
    }

    public async Task<PermissionsDto> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var dto = new PermissionsDto();
        var permissions = await _db.GetPermissionsAsync();

        if (permissions == null || permissions.Count == 0)
        {
            return dto;
        }

        dto.Permissions ??= [];

        foreach (var permission in permissions)
        {
            if (_supportedPermissions.Contains(permission.Code))
            {
                dto.Permissions.Add(new PermissionDto()
                {
                    Code = permission.Code,
                    Id = permission.Id,
                });
            }
        }
        return await Task.FromResult(dto);
    }
}
