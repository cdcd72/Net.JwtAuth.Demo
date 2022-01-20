using API.DTOs.User;

namespace API.Abstractions.Services;

public interface IUserService
{
    bool IsValidUser(string userEmail, string password, out UserDto? userDto);
}
