using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShopManagementAPI.Data;
using ShopManagementAPI.Models;
using ShopManagementAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShopManagementAPI.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _config;
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;

    public AuthController(UserManager<IdentityUser> userManager, IConfiguration config, AppDbContext context, EmailService emailService)
    {
        _userManager = userManager;
        _config = config;
        _context = context;
        _emailService = emailService;
    }

    [HttpPost("register")]
    [Consumes("application/json")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        var existing = await _userManager.FindByNameAsync(request.Username);
        if (existing != null)
            return BadRequest("Username already exists");

        var user = new IdentityUser { UserName = request.Username, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        await _userManager.AddToRoleAsync(user, "Shopkeeper");

        var shop = new Shop
        {
            UserId = user.Id,
            ShopName = request.ShopName,
            ShopAddress = request.ShopAddress,
            ContactNumber = request.ContactNumber
        };
        _context.Shops.Add(shop);
        await _context.SaveChangesAsync();

        try
        {
            await _emailService.SendAsync(
                user.Email!,
                user.UserName!,
                "Welcome to Shop Management System",
                $"Dear {user.UserName},\n\nYou have successfully registered.\nYour shop '{shop.ShopName}' has been created.\n\nPlease subscribe to a plan to start using the system.\n\nShop Management System"
            );
        }
        catch { }

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);
        return Ok(new { token });
    }

    [HttpPost("login")]
    [Consumes("application/json")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);
        return Ok(new { token });
    }

    private string GenerateJwtToken(IdentityUser user, IList<string> roles)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["DurationInMinutes"])),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class RegisterDto
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ShopName { get; set; }
    public required string ShopAddress { get; set; }
    public required string ContactNumber { get; set; }
}
