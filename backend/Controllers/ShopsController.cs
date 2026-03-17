using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopManagementAPI.Data;
using ShopManagementAPI.Models;
using System.Security.Claims;

namespace ShopManagementAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/shops")]
public class ShopsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IWebHostEnvironment _env;

    public ShopsController(AppDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment env)
    {
        _context = context;
        _userManager = userManager;
        _env = env;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var shops = await _context.Shops.ToListAsync();
        var result = new List<object>();
        foreach (var shop in shops)
        {
            var owner = await _userManager.FindByIdAsync(shop.UserId);
            result.Add(new
            {
                shop.ShopId,
                shop.ShopName,
                shop.ShopAddress,
                shop.ContactNumber,
                shop.LogoUrl,
                shop.CreatedAt,
                OwnerUsername = owner?.UserName
            });
        }
        return Ok(result);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var shop = _context.Shops.Find(id);
        if (shop == null) return NotFound("Shop not found");
        if (!User.IsInRole("Admin") && shop.UserId != userId)
            return Forbid();
        return Ok(shop);
    }

    [Authorize(Roles = "Shopkeeper")]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ShopCreateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Check active subscription
        var subscription = await _context.UserSubscriptions
            .Where(s => s.UserId == userId && s.ExpiryDate >= DateTime.Now)
            .OrderByDescending(s => s.ExpiryDate)
            .FirstOrDefaultAsync();

        if (subscription == null)
            return BadRequest("No active subscription found. Please subscribe to a plan first.");

        var plan = await _context.SubscriptionPlans.FindAsync(subscription.PlanId);
        if (plan == null)
            return BadRequest("Subscription plan not found.");

        // Check shop count against plan limit
        var shopCount = await _context.Shops.CountAsync(s => s.UserId == userId);
        if (shopCount >= plan.MaxShops)
            return BadRequest($"Your '{plan.PlanName}' plan allows a maximum of {plan.MaxShops} shop(s). Please upgrade your plan.");

        string? logoUrl = null;
        if (dto.Logo != null)
        {
            var uploadsPath = Path.Combine(_env.WebRootPath, "logos");
            Directory.CreateDirectory(uploadsPath);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Logo.FileName)}";
            using var stream = new FileStream(Path.Combine(uploadsPath, fileName), FileMode.Create);
            await dto.Logo.CopyToAsync(stream);
            logoUrl = $"/logos/{fileName}";
        }

        var shop = new Shop
        {
            UserId = userId!,
            ShopName = dto.ShopName,
            ShopAddress = dto.ShopAddress,
            ContactNumber = dto.ContactNumber,
            LogoUrl = logoUrl
        };

        _context.Shops.Add(shop);
        await _context.SaveChangesAsync();
        return Ok(shop);
    }

    [Authorize(Roles = "Shopkeeper")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] ShopCreateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var shop = _context.Shops.Find(id);
        if (shop == null) return NotFound("Shop not found");
        if (shop.UserId != userId) return Forbid();

        if (dto.Logo != null)
        {
            var uploadsPath = Path.Combine(_env.WebRootPath, "logos");
            Directory.CreateDirectory(uploadsPath);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Logo.FileName)}";
            using var stream = new FileStream(Path.Combine(uploadsPath, fileName), FileMode.Create);
            await dto.Logo.CopyToAsync(stream);
            shop.LogoUrl = $"/logos/{fileName}";
        }

        shop.ShopName = dto.ShopName;
        shop.ShopAddress = dto.ShopAddress;
        shop.ContactNumber = dto.ContactNumber;

        await _context.SaveChangesAsync();
        return Ok(shop);
    }

    [Authorize(Roles = "Shopkeeper")]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var shop = _context.Shops.Find(id);
        if (shop == null) return NotFound("Shop not found");
        if (shop.UserId != userId) return Forbid();

        _context.Shops.Remove(shop);
        _context.SaveChanges();
        return Ok("Shop deleted successfully");
    }
}

public class ShopCreateDto
{
    public required string ShopName { get; set; }
    public required string ShopAddress { get; set; }
    public required string ContactNumber { get; set; }
    public IFormFile? Logo { get; set; }
}
