using System.Text.Json.Serialization;
using API.DTOs.Role;

namespace API.DTOs.Account;

public class AccessDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("role")]
    public RoleDto Role { get; set; } = null!;

    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = null!;

    [JsonPropertyName("expiresIn")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = null!;
}
