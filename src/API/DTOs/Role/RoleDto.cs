using System.Text.Json.Serialization;

namespace API.DTOs.Role;

public class RoleDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}
