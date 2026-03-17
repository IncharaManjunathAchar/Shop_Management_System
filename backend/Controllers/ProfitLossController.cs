using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShopManagementAPI.Data;
using ShopManagementAPI.Models;
using System.Security.Claims;

namespace ShopManagementAPI.Controllers;

[Authorize(Roles = "Shopkeeper")]
[ApiController]
[Route("api/profitloss")]
public class ProfitLossController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProfitLossController(AppDbContext context)
    {
        _context = context;
    }

    private bool OwnsShop(int shopId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return _context.Shops.Any(s => s.ShopId == shopId && s.UserId == userId);
    }

    [HttpGet("{shopId}")]
    public IActionResult GetSummary(int shopId)
    {
        if (!OwnsShop(shopId)) return Forbid();
        var transactions = _context.Transactions.Where(t => t.ShopId == shopId).ToList();
        if (!transactions.Any()) return NotFound("No transactions found for this shop");
        var summary = BuildSummary(shopId, transactions, transactions.Min(t => t.TransactionDate), transactions.Max(t => t.TransactionDate));
        return Ok(summary);
    }

    [HttpGet("{shopId}/filter")]
    public IActionResult GetSummaryByDateRange(int shopId, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        if (!OwnsShop(shopId)) return Forbid();
        var transactions = _context.Transactions
            .Where(t => t.ShopId == shopId && t.TransactionDate >= from && t.TransactionDate <= to)
            .ToList();
        if (!transactions.Any()) return NotFound("No transactions found for the given date range");
        return Ok(BuildSummary(shopId, transactions, from, to));
    }

    [HttpGet("all")]
    public IActionResult GetAllShopsSummary()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var transactions = _context.Transactions
            .Where(t => _context.Shops.Any(s => s.ShopId == t.ShopId && s.UserId == userId))
            .ToList();
        var result = transactions
            .GroupBy(t => t.ShopId)
            .Select(g => BuildSummary(g.Key, g.ToList(), g.Min(t => t.TransactionDate), g.Max(t => t.TransactionDate)))
            .ToList();
        return Ok(result);
    }

    private static ProfitLossSummary BuildSummary(int shopId, List<Transaction> transactions, DateTime from, DateTime to)
    {
        var revenue = transactions
            .Where(t => t.TransactionType == "Sale")
            .Sum(t => t.TotalAmount);

        var cost = transactions
            .Where(t => t.TransactionType == "Purchase")
            .Sum(t => t.TotalAmount);

        return new ProfitLossSummary
        {
            ShopId = shopId,
            TotalRevenue = revenue,
            TotalCost = cost,
            Profit = revenue - cost,
            FromDate = from,
            ToDate = to
        };
    }
}