using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.Abstraction.Authentication;

public class LoginRequest
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    [PasswordPropertyText]
    public string Password { get; set; } = null!;
}