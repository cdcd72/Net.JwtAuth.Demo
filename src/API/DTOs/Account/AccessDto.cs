using API.DTOs.Role;

namespace API.DTOs.Account;

public class AccessDto
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public RoleDto Role { get; set; } = null!;

    public string AccessToken { get; set; } = null!;

    public int ExpiresIn { get; set; }

    public string RefreshToken { get; set; } = null!;
}
