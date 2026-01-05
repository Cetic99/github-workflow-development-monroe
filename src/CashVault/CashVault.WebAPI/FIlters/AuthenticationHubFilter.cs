using System.Security.Claims;
using CashVault.Application.Interfaces;
using CashVault.WebAPI.Common;
using Microsoft.AspNetCore.SignalR;

namespace CashVault.WebAPI.FIlters;

public class AuthenticationHubFilter : IHubFilter
{
    private readonly JwtTokenHelper _jwtTokenHelper;
    private readonly ILogger<AuthenticationHubFilter> _logger;
    private readonly ISessionService _sessionService;

    public AuthenticationHubFilter(JwtTokenHelper jwtTokenHelper,
                                   ILogger<AuthenticationHubFilter> logger,
                                   ISessionService sessionService)
    {
        _jwtTokenHelper = jwtTokenHelper;
        _logger = logger;
        _sessionService = sessionService;
    }

    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext context,
                                                      Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var httpContext = context.Context.GetHttpContext();

        var token = httpContext?.Request.Query["token"].ToString();

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                ClaimsPrincipal? claims = _jwtTokenHelper.GetClaimsFromToken(token);

                if (claims is not null)
                {
                    _jwtTokenHelper.PopulateSessionUserFromToken(_sessionService.User, claims.Claims);
                    _sessionService.AuthenticateUser();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during hub authentication.");
            }
        }

        return await next(context);
    }
}
