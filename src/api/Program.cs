using infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using domain.Entities;
using Microsoft.AspNetCore.Identity;
using application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using infrastructure.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.CodeAnalysis.Options;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
})
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

builder.Services.AddScoped<ITokenService, TokenService>();

var tokenKey = builder.Configuration["TokenSettings:TokenKey"]
    ?? throw new InvalidOperationException("TokenKey saknas i konfigurationen.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey))
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"AUTH FAILED: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = async context =>
        {
            var jti = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Jti);
            var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
            var isRevoked = await db.RevokedTokens.AnyAsync(t => t.Jti == jti);
            if (isRevoked)
            {
                context.Fail("Token är spärrad");
            }
        }
    };
});

builder.Services.AddCors();

builder.Services.AddRateLimiter (options =>
{
    options.AddFixedWindowLimiter("fixed", o =>
    {
        o.PermitLimit = 5;
        o.Window = TimeSpan.FromMinutes(1); 
        
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new IdentityRole("User"));
    }

    var adminEmail = app.Configuration["AdminSeed:Email"]
        ?? throw new InvalidOperationException("AdminSeed:Email saknas.");
    var adminPassword = app.Configuration["AdminSeed:Password"]
        ?? throw new InvalidOperationException("AdminSeed:Password saknas.");

    var result = await userManager.FindByEmailAsync(adminEmail);

    if (result is null)
    {
        var user = new User()
        {
            FirstName = "Förnamn",
            LastName = "Efternamn",
            Email = adminEmail,
            UserName = adminEmail,
            Address = new Address
            {
                StreetAddress = "Adminvägen 1",
                PostCode = 11111,
                City = "Malmö"
            }
        };

        var admin = await userManager.CreateAsync(user, adminPassword);

        if (!admin.Succeeded)
        {
            foreach (var error in admin.Errors)
            {
                Console.WriteLine($"{error.Code}: {error.Description}");
            }
        }
        else
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }

    var seed = new SeedData();
    await seed.SeedProducts(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors(builder => builder
    .AllowAnyHeader()
    .WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
    .WithMethods("GET", "POST", "PUT"));

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseRateLimiter();

app.Run();
