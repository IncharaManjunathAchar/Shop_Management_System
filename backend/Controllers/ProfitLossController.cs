using Microsoft.AspNetCore.Mvc;
using ShopManagementAPI.Data;
using ShopManagementAPI.Models;

namespace ShopManagementAPI.Controllers;

[ApiController]
[Route("api/profitloss")]
public class ProfitLossController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProfitLossController(AppDbContext context)
    {
        _context = context;
    }

    // GET api/profitloss/{shopId}
    // Overall profit/loss for a shop
    [HttpGet("{shopId}")]
    public IActionResult GetSummary(int shopId)
    {
        var transactions = _context.Transactions
            .Where(t => t.ShopId == shopId)
            .ToList();

        if (!transactions.Any())
            return NotFound("No transactions found for this shop.");

        var summary = BuildSummary(shopId, transactions, transactions.Min(t => t.TransactionDate), transactions.Max(t => t.TransactionDate));
        return Ok(summary);
    }

    // GET api/profitloss/{shopId}/filter?from=2024-01-01&to=2024-12-31
    // Profit/loss filtered by date range
    [HttpGet("{shopId}/filter")]
    public IActionResult GetSummaryByDateRange(int shopId, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var transactions = _context.Transactions
            .Where(t => t.ShopId == shopId && t.TransactionDate >= from && t.TransactionDate <= to)
            .ToList();

        if (!transactions.Any())
            return NotFound("No transactions found for the given date range.");

        var summary = BuildSummary(shopId, transactions, from, to);
        return Ok(summary);
    }

    // GET api/profitloss/all
    // Admin: profit/loss across all shops
    [HttpGet("all")]
    public IActionResult GetAllShopsSummary()
    {
        var transactions = _context.Transactions.ToList();

        var result = transactions
            .GroupBy(t => t.ShopId)
            .Select(g => BuildSummary(
                g.Key,
                g.ToList(),
                g.Min(t => t.TransactionDate),
                g.Max(t => t.TransactionDate)))
            .ToList();

        return Ok(result);
    }

    private static ProfitLossSummary BuildSummary(int shopId, List<Transaction> transactions, DateTime from, DateTime to)
    {
        var revenue = transactions.Where(t => t.TransactionType == "Sale").Sum(t => t.TotalAmount);
        var cost = transactions.Where(t => t.TransactionType == "Purchase").Sum(t => t.TotalAmount);

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
