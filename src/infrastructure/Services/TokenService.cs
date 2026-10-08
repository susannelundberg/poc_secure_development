using System.Security.Claims;
using System.Text;
using application.Interfaces;
using domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace infrastructure.Services;

public class TokenService(UserManager<User> userManager, IConfiguration config) : ITokenService
{
    public async Task<string> CreateToken(User user)
    {
        var role = await userManager.GetRolesAsync(user);

        List<Claim> claims = [
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Role, role.First()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        var tokenKey = config["tokenSettings:tokenKey"]
            ?? throw new InvalidOperationException("TokenKey saknas i konfigurationen.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var options = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(options);
    }
}
