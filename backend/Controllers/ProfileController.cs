using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopManagementAPI.Data;
using System.Security.Claims;

namespace ShopManagementAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ProfileController(UserManager<IdentityUser> userManager, AppDbContext context, IWebHostEnvironment env)
    {
        _userManager = userManager;
        _context = context;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        var shop = _context.Shops.FirstOrDefault(s => s.UserId == userId);
        var subscription = _context.UserSubscriptions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.SubscriptionId)
            .Select(s => new { s.Status, s.ExpiryDate, PlanName = s.Plan != null ? s.Plan.PlanName : null, s.SubscriptionId })
            .FirstOrDefault();

        return Ok(new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.PhoneNumber,
            Role = roles.FirstOrDefault(),
            Shop = shop == null ? null : new
            {
                shop.ShopId, shop.ShopName, shop.ShopAddress,
                shop.ContactNumber, shop.LogoUrl, shop.CreatedAt
            },
            Subscription = subscription
        });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            user.PhoneNumber = dto.PhoneNumber;

        await _userManager.UpdateAsync(user);

        var shop = _context.Shops.FirstOrDefault(s => s.UserId == userId);
        if (shop != null)
        {
            if (!string.IsNullOrWhiteSpace(dto.ShopName))    shop.ShopName = dto.ShopName;
            if (!string.IsNullOrWhiteSpace(dto.ShopAddress)) shop.ShopAddress = dto.ShopAddress;
            if (!string.IsNullOrWhiteSpace(dto.ContactNumber)) shop.ContactNumber = dto.ContactNumber;

            if (dto.Logo != null)
            {
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var ext = Path.GetExtension(dto.Logo.FileName).ToLower();
                if (!allowed.Contains(ext)) return BadRequest("Only image files are allowed.");
                if (dto.Logo.Length > 2 * 1024 * 1024) return BadRequest("Logo must be under 2MB.");

                var uploadsPath = Path.Combine(_env.WebRootPath, "logos");
                Directory.CreateDirectory(uploadsPath);
                var fileName = $"{Guid.NewGuid()}{ext}";
                using var stream = new FileStream(Path.Combine(uploadsPath, fileName), FileMode.Create);
                await dto.Logo.CopyToAsync(stream);
                var logoUrl = $"/logos/{fileName}";

                // Apply same logo to ALL shops of this user
                var allShops = _context.Shops.Where(s => s.UserId == userId).ToList();
                foreach (var s in allShops) s.LogoUrl = logoUrl;
                shop.LogoUrl = logoUrl;
            }

            await _context.SaveChangesAsync();
        }

        return Ok(new { message = "Profile updated successfully.", logoUrl = shop?.LogoUrl });
    }

    [Authorize(Roles = "Shopkeeper")]
    [HttpPut("cancel-subscription")]
    public async Task<IActionResult> CancelSubscription()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var subscription = _context.UserSubscriptions
            .Where(s => s.UserId == userId && (s.Status == "Approved" || s.Status == "Pending"))
            .OrderByDescending(s => s.SubscriptionId)
            .FirstOrDefault();

        if (subscription == null) return NotFound("No active subscription found.");

        subscription.Status = "Cancelled";
        await _context.SaveChangesAsync();
        return Ok(new { message = "Subscription cancelled." });
    }
}

public class UpdateProfileDto
{
    public string? PhoneNumber { get; set; }
    public string? ShopName { get; set; }
    public string? ShopAddress { get; set; }
    public string? ContactNumber { get; set; }
    public IFormFile? Logo { get; set; }
}
