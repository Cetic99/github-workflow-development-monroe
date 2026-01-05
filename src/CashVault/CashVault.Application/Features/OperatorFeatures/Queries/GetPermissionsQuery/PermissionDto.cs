using CashVault.Domain.Aggregates.OperatorAggregate;

namespace CashVault.Application.Features.OperatorFeatures.Queries.GetPermissionsQuery;

public class PermissionDto
{
    public int Id { get; set; }
    public string Code { get; set; }
}