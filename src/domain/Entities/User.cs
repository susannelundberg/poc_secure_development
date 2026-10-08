using Microsoft.AspNetCore.Identity;

namespace domain.Entities;

public class User : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required Address Address { get; set; }
}
