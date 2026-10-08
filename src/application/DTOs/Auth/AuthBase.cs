using System.ComponentModel.DataAnnotations;

namespace application.DTOs.Auth;

public class AuthBase
{
    [EmailAddress]
    public required string Email { get; set; }
    public required string Password { get; set; }
}
