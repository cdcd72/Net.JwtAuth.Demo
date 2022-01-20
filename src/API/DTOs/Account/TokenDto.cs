using System.Text.Json.Serialization;

namespace API.DTOs.Account;

public class TokenDto
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = null!;

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = null!;
}
