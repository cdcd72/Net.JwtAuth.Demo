using API.DTOs.Role;

namespace API.DTOs.User;

public class UserDto
{
    public Guid Id { get; set; }

    public string Email { get; set; }

    public RoleDto Role { get; set; }
}
