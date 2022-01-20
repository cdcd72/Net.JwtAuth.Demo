using API.DTOs.Account;

namespace API.Abstractions.Helpers.Auth.Authenticators;

public interface IAuthenticator
{
    int AccessTokenExpirationMinutes { get; set; }

    TokenDto GenerateToken(Guid userId, string userEmail, string userRole);

    TokenDto RefreshToken(string refreshToken, string? accessToken);

    bool ValidateRefreshToken(string refreshToken);

    void DeleteRefreshToken(Guid userId);

    void AddTokenToBlackList(string? accessToken);
}
