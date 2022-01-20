using Microsoft.Extensions.Options;

namespace API.Helpers.Auth.Token;

public class RefreshTokenHelper : TokenHelper
{
    private readonly JwtConfiguration jwtConfig;

    public RefreshTokenHelper(IOptions<JwtConfiguration> jwtConfig) => this.jwtConfig = jwtConfig.Value;

    public string GenerateToken()
        => base.GenerateToken(
            jwtConfig.RefreshTokenSecret,
            jwtConfig.Issuer,
            jwtConfig.Audience,
            jwtConfig.RefreshTokenExpirationMinutes);

    public bool ValidateToken(string token)
        => base.ValidateToken(
            jwtConfig.RefreshTokenSecret,
            jwtConfig.Issuer,
            jwtConfig.Audience,
            token);
}
