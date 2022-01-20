using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Account;

public class RefreshDto
{
    [Required]
    public string RefreshToken { get; set; }
}
