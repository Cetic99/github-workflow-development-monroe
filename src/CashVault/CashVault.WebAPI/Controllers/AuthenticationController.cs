using System.Security.Claims;
using System.Text.Json;
using CashVault.Application.Features.OperatorFeatures.Commands;
using CashVault.Application.Interfaces;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Infrastructure.Configuration;
using CashVault.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CashVault.WebAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ISessionService _sessionService;
        private readonly IOperatorRepository _operatorRepository;
        private readonly JWTSettings _jWTSettings;
        private readonly JwtTokenHelper _jwtTokenHelper;

        public AuthenticationController(ILogger<AccountController> logger,
                                        IOptions<JWTSettings> jwtOptions,
                                        ISessionService sessionService,
                                        IOperatorRepository operatorRepository,
                                        JwtTokenHelper jwtTokenHelper)
        {
            _logger = logger;
            _jWTSettings = jwtOptions.Value;
            _sessionService = sessionService;
            _operatorRepository = operatorRepository;
            _jwtTokenHelper = jwtTokenHelper;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateOperatorWithCredentialsCommand request)
        {
            await Mediator.Send(request);
            return Accepted();
        }

        [HttpPost("logout")]
        public async Task<Unit> Logout()
        {
            return await Mediator.Send(new LogoutCommand());
        }

        [Authorize]
        [HttpPost("refresh/{token}")]
        public async Task<IActionResult> Refresh([FromRoute] string token)
        {
            var operatorId = 0;
            var secret = _jWTSettings.Secret;
            var accessValidity = _jWTSettings.AccessTokenValidityInSeconds;
            var refreshValidity = _jWTSettings.RefreshTokenValidityInSeconds;

            var jwtToken = _jwtTokenHelper.ValidateRefreshToken(token, secret);

            if (jwtToken == null) return Unauthorized();

            try
            {
                operatorId = int.Parse(
                    jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier.ToString()).Value
                );
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }

            if (_sessionService.User.Id != operatorId) return Unauthorized();

            var @operator = await _operatorRepository.GetOperatorWithPermissionsAsync(operatorId);
            var claims = jwtToken.Claims.ToList();
            var claimToRemove = claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Permisssions);
            claims.Remove(claimToRemove);

            if (@operator.Permissions.Any())
                claims.Add
                    (new Claim(CustomClaimTypes.Permisssions,
                     JsonSerializer.Serialize(@operator.Permissions.Select(x => x.Code).ToList())));

            var accessToken =
                JwtTokenHelper.GenerateJwtToken
                    (JwtTokenType.Access, secret, DateTime.UtcNow.AddSeconds(accessValidity), claims);

            var refreshToken =
                JwtTokenHelper.GenerateJwtToken
                    (JwtTokenType.Refresh, secret, DateTime.UtcNow.AddSeconds(refreshValidity), claims);

            return Ok(new { accessToken, refreshToken });
        }
    }
}
