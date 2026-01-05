using CashVault.Application.Features.OperatorFeatures.Queries.GetPermissionsQuery;
using CashVault.Application.Interfaces.Persistence;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.OperatorFeatures.Queries.GetOperatorsQuery;

public class GetOperatorsQueryValidator : AbstractValidator<GetOperatorsQuery>
{
    public GetOperatorsQueryValidator()
    {
        RuleFor(x => x.Page).NotEmpty().GreaterThan(0).WithMessage("Page is required.");
        RuleFor(x => x.PageSize).NotEmpty().GreaterThan(0).WithMessage("PageSize is required.");
    }
}

public record GetOperatorsQuery : IRequest<OperatorsDto>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

internal sealed class GetOperatorsQueryHandler : IRequestHandler<GetOperatorsQuery, OperatorsDto>
{
    private readonly IOperatorRepository _db;

    public GetOperatorsQueryHandler(IOperatorRepository db)
    {
        _db = db;
    }

    public async Task<OperatorsDto> Handle(GetOperatorsQuery request, CancellationToken cancellationToken)
    {
        var dto = new OperatorsDto();
        var operators = await _db.GetOperatorsWithPermissionsAsync(
            request.Page,
            request.PageSize,
            request.FirstName,
            request.LastName);

        if (operators?.Data?.Any() ?? false)
        {
            dto.Page = operators.Page;
            dto.PageSize = operators.PageSize;
            dto.TotalCount = operators.TotalCount;

            dto.Operators = operators.Data.Select(x => new OperatorDto()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                PhoneNumber = x.PhoneNumber,
                Username = x.Username,
                Remarks = x.Remarks,
                Email = x.Email,
                IsActive = x.IsActive,
                Company = x?.Company,
                Permissions = x.Permissions.Select(x => new PermissionDto() { Id = x.Id, Code = x.Code }).ToList()
            }).ToList();
        }

        return await Task.FromResult(dto);
    }
}
