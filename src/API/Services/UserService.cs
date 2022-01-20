using API.Abstractions.Services;
using API.DTOs.Role;
using API.DTOs.User;
using API.Entities;
using API.Helpers.Auth;

namespace API.Services;

public class UserService : IUserService
{
    private readonly IDictionary<string, User> users = new Dictionary<string, User>
    {
        {
            "test@example.com",
            new User
            {
                Id = new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"),
                Email = "test@gmail.com",
                Password = "test",
                Role = new Role
                {
                    Id = new Guid("fb2bd817-98cd-4cf3-a80a-53ea0cd9c20f"),
                    Name = RoleConstant.Normal
                }
            }
        },
        {
            "admin@example.com",
            new User
            {
                Id = new Guid("cb2bd887-98cd-4cf3-a86a-53ea0cd9c200"),
                Email = "admin@gmail.com",
                Password = "securePa55",
                Role = new Role
                {
                    Id = new Guid("eb2bd817-98cd-4cf3-a80a-53ea0cd9c20e"),
                    Name = RoleConstant.Administrator
                }
            }
        }
    };

    public bool IsValidUser(string userEmail, string password, out UserDto? userDto)
    {
        userDto = null;

        if (!users.TryGetValue(userEmail, out var user))
            return false;

        var isValidUser = user.Password == password;

        if(isValidUser)
            userDto = GetUser(userEmail);

        return isValidUser;

    }

    private UserDto? GetUser(string userEmail)
    {
        if (users.TryGetValue(userEmail, out var user))
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = new RoleDto
                {
                    Id = user.Role.Id,
                    Name = user.Role.Name
                }
            };
        }

        return null;
    }
}
