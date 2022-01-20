namespace API.Helpers.Auth;

public class JwtConfiguration
{
    public const string SectionName = "JwtConfig";

    public string AccessTokenSecret { get; set; } = null!;

    public int AccessTokenExpirationMinutes { get; set; }

    public string RefreshTokenSecret { get; set; } = null!;

    public int RefreshTokenExpirationMinutes { get; set; }

    public string Issuer { get; set; } = null!;

    public string Audience { get; set; } = null!;
}
