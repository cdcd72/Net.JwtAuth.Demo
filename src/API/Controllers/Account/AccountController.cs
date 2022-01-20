using System.Security.Claims;
using API.Abstractions.Helpers.Auth.Authenticators;
using API.Abstractions.Services;
using API.DTOs.Account;
using API.DTOs.Role;
using API.Helpers.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Account;

public class AccountController : BaseApiController
{
    private readonly IUserService userService;
    private readonly IAuthenticator authenticator;

    #region Constructor

    public AccountController(
        IUserService userService,
        IAuthenticator authenticator)
    {
        this.userService = userService;
        this.authenticator = authenticator;
    }

    #endregion

    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult Login([FromBody] LoginDto loginInfo)
    {
        if (!userService.IsValidUser(loginInfo.Email, loginInfo.Password, out var user))
            return BadRequest("Invalid password!");

        if (user is null)
            return BadRequest("User not found!");

        var tokenDto = authenticator.GenerateToken(user.Id, user.Email, user.Role.Name);

        return Ok(new AccessDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = new RoleDto
            {
                Id = user.Role.Id,
                Name = user.Role.Name
            },
            AccessToken = tokenDto.AccessToken,
            ExpiresIn = authenticator.AccessTokenExpirationMinutes * 60 * 1000,
            RefreshToken = tokenDto.RefreshToken,
        });
    }

    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto refreshInfo)
    {
        if (!authenticator.ValidateRefreshToken(refreshInfo.RefreshToken))
            return BadRequest("Invalid refresh token!");

        var accessToken =
            await HttpContext.GetTokenAsync(JwtBearerDefaults.AuthenticationScheme, "access_token");

        var tokenDto = authenticator.RefreshToken(refreshInfo.RefreshToken, accessToken);

        authenticator.AddTokenToBlackList(accessToken);

        return Ok(tokenDto);
    }

    [Authorize]
    [HttpDelete("logout")]
    public async Task<ActionResult> Logout()
    {
        if (!Guid.TryParse(HttpContext.User.FindFirstValue(CustomClaimTypes.Id), out var userId))
            return BadRequest("Invalid user id!");

        authenticator.DeleteRefreshToken(userId);

        var accessToken =
            await HttpContext.GetTokenAsync(JwtBearerDefaults.AuthenticationScheme, "access_token");

        authenticator.AddTokenToBlackList(accessToken);

        return Ok();
    }
}
