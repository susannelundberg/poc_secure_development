using System.ComponentModel.DataAnnotations;
using domain.Entities;

namespace application.DTOs.Auth;

public class RegisterUserDTO : AuthBase
{
    [StringLength(20, MinimumLength = 1)]
    public required string FirstName { get; set; }
    [StringLength(50, MinimumLength = 1)]
    public required string LastName { get; set; }
    [StringLength(30, MinimumLength = 1)]
    public required string StreetAddress { get; set; }
    [Range(10000, 99999)]
    public required int PostCode { get; set; }
    [StringLength(20, MinimumLength = 1)]
    public required string City { get; set; }
}
