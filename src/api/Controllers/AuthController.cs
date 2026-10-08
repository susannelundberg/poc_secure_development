using domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using application.Interfaces;
using application.DTOs.Auth;
using System.Security.Claims;
using infrastructure.Data;
using Microsoft.AspNetCore.RateLimiting;

namespace api.Controllers;

[Authorize]
[Route("api/auth")]
[ApiController]
public class AuthController(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService, AppDbContext context) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> RegisterUser(RegisterUserDTO model)
    {
        try
        {
            User user = new()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email,
                Email = model.Email,
                Address = new Address
                {
                    StreetAddress = model.StreetAddress,
                    PostCode = model.PostCode,
                    City = model.City
                }
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new { Success = false, message = "Kontot kunde inte skapas" });
            }

            await userManager.AddToRoleAsync(user, "User");

            return StatusCode(201, new { Success = true, message = $"Du har nu skapat ett konto för {user.Email}" });
        }
        catch
        {
            return StatusCode(500, "Kontot kunde inte skapas");
        }

    }

    [AllowAnonymous]
    [EnableRateLimiting ("fixed")]
    [HttpPost("login")] // KOLLA PÅ DETTA
    // [Route("api/login")]
    public async Task<ActionResult> LoginUser(LoginUserDTO model)
    {
        try
        {
            var user = await userManager.FindByNameAsync(model.Email);

            if (user is null)
            {
                return Unauthorized(new { Success = false, message = "Felaktig inloggning, kontrollera användarnamn och lösenord." });
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                return Unauthorized(new { Success = false, message = "Felaktig inloggning, kontrollera användarnamn och lösenord." });
            }

            var token = await tokenService.CreateToken(user);
            var response = await userManager.GetRolesAsync(user);
            var role = response.First();

            return Ok(new { Success = true, user.Email, token, role });
        }
        catch
        {
            return StatusCode(500, "Något gick fel");
        }
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        try
        {
            var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
            if (jti is null) return Unauthorized();

            var revokedToken = new RevokedToken()
            {
                Jti = jti
            };

            context.RevokedTokens.Add(revokedToken);
            await context.SaveChangesAsync();

            return Ok(new { Success = true, message = "Du är utloggad" });
        }
        catch
        {
            return StatusCode(500, "Något gick fel");
        }
    }
}