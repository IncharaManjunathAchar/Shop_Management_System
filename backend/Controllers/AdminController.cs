using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShopManagementAPI.Data;

namespace ShopManagementAPI.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("dashboard")]
    public IActionResult GetDashboardStats()
    {
        var totalShops = _context.Shops.Count();
        var activeSubscriptions = _context.UserSubscriptions
            .Count(s => s.ExpiryDate >= DateTime.Now);

        return Ok(new
        {
            TotalShops = totalShops,
            ActiveSubscriptions = activeSubscriptions
        });
    }
}