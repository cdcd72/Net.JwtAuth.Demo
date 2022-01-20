using System.Security.Claims;
using API.Abstractions.Helpers.Auth.Authenticators;
using API.Common;
using API.DTOs.Account;
using API.Helpers.Auth.Token;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace API.Helpers.Auth.Authenticators;

public class JwtAuthenticator : IAuthenticator
{
    private readonly AccessTokenHelper accessTokenHelper;
    private readonly RefreshTokenHelper refreshTokenHelper;
    private readonly IMemoryCache cache;

    #region Properties

    public int AccessTokenExpirationMinutes { get; set; }

    #endregion

    public JwtAuthenticator(
        AccessTokenHelper accessTokenHelper,
        RefreshTokenHelper refreshTokenHelper,
        IMemoryCache cache)
    {
        this.accessTokenHelper = accessTokenHelper;
        this.refreshTokenHelper = refreshTokenHelper;
        this.cache = cache;

        AccessTokenExpirationMinutes = this.accessTokenHelper.ExpirationMinutes;
    }

    public TokenDto GenerateToken(Guid userId, string userEmail, string userRole)
    {
        var accessToken = accessTokenHelper.GenerateToken(userId, userEmail, userRole);
        var refreshToken = refreshTokenHelper.GenerateToken();

        // Refresh token can save in database or distributed cache, but use memory cache for demo...
        // This situation represent one user can hold one refresh token...
        cache.Set(userId, refreshToken);

        return new TokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public bool ValidateRefreshToken(string refreshToken)
        => refreshTokenHelper.ValidateToken(refreshToken);

    public TokenDto RefreshToken(string refreshToken, string? accessToken)
    {
        var (principal, token) = accessTokenHelper.DecodeToken(accessToken);

        if (token?.Header.Alg is not SecurityAlgorithms.HmacSha256)
            throw new SecurityTokenException("Invalid token algorithm!");

        if (!Guid.TryParse(principal.FindFirstValue(CustomClaimTypes.Id), out var userId))
            throw new SecurityTokenException("Invalid user id!");

        var userEmail = principal.FindFirstValue(ClaimTypes.Email);
        var userRole = principal.FindFirstValue(ClaimTypes.Role);

        if (!cache.TryGetValue(userId, out string existingRefreshToken))
            throw new SecurityTokenException("Refresh token not found!");

        if (refreshToken != existingRefreshToken)
            throw new SecurityTokenException("Invalid token!");

        return GenerateToken(userId, userEmail, userRole);
    }

    public void DeleteRefreshToken(Guid userId)
    {
        if (cache.TryGetValue(userId, out string _))
            cache.Remove(userId);
    }

    public void AddTokenToBlackList(string? accessToken)
    {
        cache.TryGetValue(CacheKeys.TokenBlackList, out List<string?> tokenBlackList);

        tokenBlackList ??= new List<string?>();

        tokenBlackList.Add(accessToken);

        cache.Remove(CacheKeys.TokenBlackList);

        cache.Set(CacheKeys.TokenBlackList, tokenBlackList,
            TimeSpan.FromMinutes(AccessTokenExpirationMinutes));
    }
}
