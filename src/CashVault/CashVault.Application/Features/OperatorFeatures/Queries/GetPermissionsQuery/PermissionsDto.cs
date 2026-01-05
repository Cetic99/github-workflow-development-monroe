using CashVault.Domain.Aggregates.OperatorAggregate;

namespace CashVault.Application.Features.OperatorFeatures.Queries.GetPermissionsQuery;

public class PermissionsDto
{
    public List<PermissionDto> Permissions { get; set; }
}