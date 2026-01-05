using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using CashVault.Application.Common.Models;
using CashVault.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CashVault.WebAPI.Common;

public class JwtTokenHelper
{
    private readonly JWTSettings _jwtConfiguration;
    private readonly ILogger<JwtTokenHelper> _logger;
    private const string TOKEN_TYPE_CLAIM_NAME = "TokenType";
    private const int CLOCK_SKEW_SECONDS = 15;

    public JwtTokenHelper(IOptions<JWTSettings> jwtConfigurationOptions, ILogger<JwtTokenHelper> logger)
    {
        _jwtConfiguration = jwtConfigurationOptions.Value;
        _logger = logger;
    }

    public static string GenerateJwtToken(JwtTokenType type, string secret, DateTime expires, List<Claim> claims)
    {
        var _claims = new List<Claim>(claims)
        {
            new(TOKEN_TYPE_CLAIM_NAME, type.Code)
        };

        var jwtToken = new JwtSecurityToken(
            claims: _claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials:
                new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    SecurityAlgorithms.HmacSha256Signature)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }

    public JwtSecurityToken? ValidateRefreshToken(string refreshToken, string secret)
    {
        JwtSecurityToken? jwtToken = null;
        var key = Encoding.UTF8.GetBytes(secret);

        var handler = new JwtSecurityTokenHandler();
        var symetricSecurityKey = new SymmetricSecurityKey(key);

        try
        {
            handler.ValidateToken(refreshToken, new TokenValidationParameters()
            {
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.FromSeconds(CLOCK_SKEW_SECONDS)
            },
            out SecurityToken outToken);

            jwtToken = outToken as JwtSecurityToken;

            var tokenTypeClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == TOKEN_TYPE_CLAIM_NAME);

            if (tokenTypeClaim == null)
            {
                _logger.LogError("TokenType claim is missing in the refresh token.");
                return null;
            }

            var tokenType = tokenTypeClaim.Value;

            if (tokenType != JwtTokenType.Refresh.Code) return null;
        }
        catch (Exception e)
        {
            return null;
        }

        return jwtToken;
    }

    public ClaimsPrincipal? GetClaimsFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Secret));
        ClaimsPrincipal claimsPrincipal;

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.FromSeconds(CLOCK_SKEW_SECONDS)
        };

        try
        {
            claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
        }
        catch (Exception)
        {
            return null;
        }

        return claimsPrincipal;
    }

    public void PopulateSessionUserFromToken(SessionUser sessionUser, IEnumerable<Claim> claims)
    {

        Claim? idClaim = claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
        Claim? usernameClaim = claims.Where(x => x.Type == ClaimTypes.Name).FirstOrDefault();
        Claim? permissionsClaim = claims.Where(x => x.Type == CustomClaimTypes.Permisssions).FirstOrDefault();
        Claim? fullNameClaim = claims.Where(x => x.Type == CustomClaimTypes.UserFullName).FirstOrDefault();
        Claim? companyClaim = claims.Where(x => x.Type == CustomClaimTypes.UserCompany).FirstOrDefault();

        if (idClaim != null)
        {
            sessionUser.Id = int.Parse(idClaim!.Value);
        }

        if (usernameClaim != null)
        {
            sessionUser.Username = usernameClaim!.Value;
        }

        if (fullNameClaim != null)
        {
            sessionUser.FullName = fullNameClaim!.Value;
        }

        if (companyClaim != null)
        {
            sessionUser.Company = companyClaim!.Value;
        }

        if (permissionsClaim != null && permissionsClaim?.Value != null)
        {
            try
            {
                var permissions = JsonSerializer.Deserialize<List<string>>(permissionsClaim.Value);

                if (permissions != null)
                {
                    sessionUser.Permissions = permissions;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing permissions from token.");
            }
        }
    }
}