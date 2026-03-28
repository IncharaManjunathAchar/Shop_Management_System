using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopManagementAPI.Data;

namespace ShopManagementAPI.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(AppDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var totalShops = _context.Shops.Count();
        var activeSubscriptions = _context.UserSubscriptions
            .Count(s => s.Status == "Approved" && s.ExpiryDate >= DateTime.Now);
        var totalUsers = (await _userManager.GetUsersInRoleAsync("Shopkeeper")).Count;
        var pendingSubscriptions = _context.UserSubscriptions.Count(s => s.Status == "Pending");
        var expiringSoon = _context.UserSubscriptions
            .Count(s => s.Status == "Approved" && s.ExpiryDate >= DateTime.Now && s.ExpiryDate <= DateTime.Now.AddDays(7));
        var totalRevenue = _context.UserSubscriptions
            .Where(s => s.Status == "Approved")
            .Join(_context.SubscriptionPlans, us => us.PlanId, p => p.PlanId, (us, p) => p.Price)
            .Sum();

        return Ok(new
        {
            TotalShops = totalShops,
            ActiveSubscriptions = activeSubscriptions,
            TotalUsers = totalUsers,
            PendingSubscriptions = pendingSubscriptions,
            ExpiringSoon = expiringSoon,
            TotalRevenue = totalRevenue
        });
    }

    [HttpGet("subscriptions")]
    public IActionResult GetAllSubscriptions()
    {
        var subs = _context.UserSubscriptions
            .Select(s => new
            {
                s.SubscriptionId,
                s.UserId,
                Username = s.User != null ? s.User.UserName : null,
                s.PlanId,
                PlanName = s.Plan != null ? s.Plan.PlanName : null,
                Price = s.Plan != null ? s.Plan.Price : 0,
                s.StartDate,
                s.ExpiryDate,
                s.Status,
                s.PaymentStatus
            })
            .OrderByDescending(s => s.SubscriptionId)
            .ToList();
        return Ok(subs);
    }

    [HttpGet("shops")]
    public async Task<IActionResult> GetShopsWithOwners()
    {
        var shops = await _context.Shops
            .Include(s => s.User)
            .Select(s => new
            {
                s.ShopId,
                s.ShopName,
                s.ShopAddress,
                s.ContactNumber,
                s.LogoUrl,
                s.CreatedAt,
                Owner = new
                {
                    s.UserId,
                    Username = s.User != null ? s.User.UserName : null,
                    Email = s.User != null ? s.User.Email : null
                }
            })
            .ToListAsync();

        return Ok(shops);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var shopkeepers = await _userManager.GetUsersInRoleAsync("Shopkeeper");

        var result = shopkeepers.Select(u => new
        {
            u.Id,
            u.UserName,
            u.Email,
            Subscription = _context.UserSubscriptions
                .Where(s => s.UserId == u.Id)
                .OrderByDescending(s => s.SubscriptionId)
                .Select(s => new { s.Status, s.ExpiryDate, PlanName = s.Plan != null ? s.Plan.PlanName : null })
                .FirstOrDefault()
        });

        return Ok(result);
    }
}