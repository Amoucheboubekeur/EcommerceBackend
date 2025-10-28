using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.DTOs.Users;
public class LoginDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

