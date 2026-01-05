using System.Net;
using System.Security.Claims;
using CashVault.Application.Interfaces;
using CashVault.WebAPI.Common;

namespace CashVault.WebAPI.Middleware
{
    /// <summary>
    /// Class for handling session parameters.
    /// </summary>
    public class SessionMiddleware
    {
        /// <summary>
        /// The <c>SessionMiddleware</c> is middleware where we fill user information for current session.
        /// </summary>
        private readonly RequestDelegate _next;
        private readonly JwtTokenHelper _jwtTokenHelper;

        /// <summary>
        /// Construcor of session.
        /// </summary>
        /// <param name="next"> Delegate on next action. </param>
        /// <param name="jwtTokenHelper"> JWT token helper for populating user data from token. </param>
        public SessionMiddleware(RequestDelegate next,
                                 JwtTokenHelper jwtTokenHelper)
        {
            _next = next;
            _jwtTokenHelper = jwtTokenHelper;
        }

        /// <summary>
        /// Method called on invoke of action.
        /// </summary>
        /// <param name="context"> Context of application. </param>
        /// <param name="sessionService"> Session service that will be populated. </param>
        /// <returns> Awaiter for next task with necessary session data. </returns>
        public async Task Invoke(HttpContext context, ISessionService sessionService)
        {
            string clientIpAddress = GetClientIpAddress(context);

            if (IPAddress.TryParse(clientIpAddress, out IPAddress parsedIpAddress) && IPAddress.IsLoopback(parsedIpAddress))
            {
                clientIpAddress = "localhost";
            }

            sessionService.IPAddress = clientIpAddress;
            var authorizationHeader = context.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrWhiteSpace(authorizationHeader) &&
                authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authorizationHeader["Bearer ".Length..].Trim();

                ClaimsPrincipal? claims = _jwtTokenHelper.GetClaimsFromToken(token);

                if (claims is not null)
                {
                    _jwtTokenHelper.PopulateSessionUserFromToken(sessionService.User, claims.Claims);
                    sessionService.AuthenticateUser();
                }
            }

            if (context.Request.Headers.TryGetValue("Accept-Language", out var language))
            {
                sessionService.Language = language;
            }

            await _next(context);
        }

        #region PRIVATE
        private static string GetClientIpAddress(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Real-IP", out var realIpValues) && realIpValues.Count > 0)
            {
                return realIpValues.First();
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        }
        #endregion
    }
}
