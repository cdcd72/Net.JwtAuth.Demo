using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace API.Helpers.Auth.Token;

public class AccessTokenHelper : TokenHelper
{
    private readonly JwtConfiguration jwtConfig;

    #region Properties

    public int ExpirationMinutes => jwtConfig.AccessTokenExpirationMinutes;

    #endregion

    public AccessTokenHelper(IOptions<JwtConfiguration> jwtConfig) => this.jwtConfig = jwtConfig.Value;

    public string GenerateToken(Guid userId, string userEmail, string userRole)
    {
        var claims = new List<Claim>
        {
            new(CustomClaimTypes.Id, $"{userId}"),
            new(ClaimTypes.Email, userEmail),
            new(ClaimTypes.Role, GetApiRole(userRole))
        };

        return base.GenerateToken(
            jwtConfig.AccessTokenSecret,
            jwtConfig.Issuer,
            jwtConfig.Audience,
            jwtConfig.AccessTokenExpirationMinutes,
            claims);
    }

    public (ClaimsPrincipal, JwtSecurityToken?) DecodeToken(string? token)
        => base.DecodeToken(
            jwtConfig.AccessTokenSecret,
            jwtConfig.Issuer,
            jwtConfig.Audience,
            token);

    #region Private Method

    private static string GetApiRole(string role)
    {
        var adminRoles = new[] { RoleConstant.Administrator };

        return adminRoles.Contains(role) ? AuthConstant.Admin : AuthConstant.User;
    }

    #endregion
}
