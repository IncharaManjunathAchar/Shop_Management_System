using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // 🔥 ADD THIS
using ShopManagementAPI.Data;

namespace ShopManagementAPI.Controllers;

[Authorize] // 🔐 ADD THIS
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
        var totalTransactions = _context.Transactions.Count();
        var activeSubscriptions = _context.UserSubscriptions
            .Count(s => s.ExpiryDate >= DateTime.Now);

        var totalRevenue = _context.Transactions
            .Where(t => t.TransactionType == "Sale")
            .Sum(t => (decimal?)t.TotalAmount) ?? 0;

        var totalCost = _context.Transactions
            .Where(t => t.TransactionType == "Purchase")
            .Sum(t => (decimal?)t.TotalAmount) ?? 0;

        return Ok(new
        {
            TotalShops = totalShops,
            TotalTransactions = totalTransactions,
            ActiveSubscriptions = activeSubscriptions,
            TotalRevenue = totalRevenue,
            TotalCost = totalCost,
            NetProfit = totalRevenue - totalCost
        });
    }
}