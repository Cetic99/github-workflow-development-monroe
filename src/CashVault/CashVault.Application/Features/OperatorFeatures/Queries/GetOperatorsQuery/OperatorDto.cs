using CashVault.Application.Features.OperatorFeatures.Queries.GetPermissionsQuery;

namespace CashVault.Application.Features.OperatorFeatures.Queries.GetOperatorsQuery;

public class OperatorDto
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Username { get; set; }
    public string? Remarks { get; set; }
    public string? Email { get; set; }
    public bool? IsActive { get; set; }
    public string? Company { get; set; }
    public List<PermissionDto> Permissions { get; set; }
}