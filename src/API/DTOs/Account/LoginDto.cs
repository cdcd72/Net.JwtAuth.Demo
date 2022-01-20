using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Account;

public class LoginDto
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address!")]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
