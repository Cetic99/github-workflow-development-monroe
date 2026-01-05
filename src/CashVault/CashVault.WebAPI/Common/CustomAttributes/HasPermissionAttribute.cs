using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.OperatorAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CashVault.WebAPI.Common.CustomAttributes
{
    public class HasPermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly List<string> _permissions;

        public HasPermissionAttribute(params string[] permissions)
        {
            _permissions = permissions?.Select(x => x.ToLowerInvariant()).ToList() ?? new List<string>();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var sessionService = context.HttpContext.RequestServices.GetService<ISessionService>();

            if (sessionService == null || !sessionService.IsAuthenticated || !_permissions.Any())
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userPermissions =
                sessionService.User.Permissions.Select(x => x.ToLowerInvariant()).ToList();

            if (userPermissions == null || !_permissions.Any(userPermissions.Contains))
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
