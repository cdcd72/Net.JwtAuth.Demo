using API.DTOs.Role;

namespace API.DTOs.Account;

public class AccessDto
{
    public Guid Id { get; set; }

    public string Email { get; set; }

    public RoleDto Role { get; set; }

    public string AccessToken { get; set; }

    public int ExpiresIn { get; set; }

    public string RefreshToken { get; set; }
}
