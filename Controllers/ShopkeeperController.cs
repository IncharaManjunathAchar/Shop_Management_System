using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopManagementAPI.Data;
using ShopManagementAPI.Models;

namespace ShopManagementAPI.Controllers;

[ApiController]
[Route("api/shop")]
public class ShopController : ControllerBase
{
    private readonly AppDbContext _context;

    public ShopController(AppDbContext context)
    {
        _context = context;
    }

    // ---------------- ITEMS ----------------

    // Get all items for a shop
    [HttpGet("{shopId}/items")]
    public IActionResult GetItems(int shopId)
    {
        var items = _context.Items
            .Where(i => i.ShopId == shopId)
            .ToList();

        return Ok(items);
    }

    // Get item by id
    [HttpGet("items/{id}")]
    public IActionResult GetItem(int id)
    {
        var item = _context.Items.FirstOrDefault(i => i.ItemId == id);

        if (item == null)
            return NotFound();

        return Ok(item);
    }

    // Add new item
    [HttpPost("{shopId}/items")]
    public IActionResult AddItem(int shopId, Item item)
    {
        item.ShopId = shopId;

        _context.Items.Add(item);
        _context.SaveChanges();

        return Ok(item);
    }

    // Update item
    [HttpPut("items/{id}")]
    public IActionResult UpdateItem(int id, Item updatedItem)
    {
        var item = _context.Items.FirstOrDefault(i => i.ItemId == id);

        if (item == null)
            return NotFound();

        item.ItemName = updatedItem.ItemName;
        item.Quantity = updatedItem.Quantity;
        item.CostPrice = updatedItem.CostPrice;
        item.SellingPrice = updatedItem.SellingPrice;
        item.ExpiryDate = updatedItem.ExpiryDate;

        _context.SaveChanges();

        return Ok(item);
    }

    // Delete item
    [HttpDelete("items/{id}")]
    public IActionResult DeleteItem(int id)
    {
        var item = _context.Items.FirstOrDefault(i => i.ItemId == id);

        if (item == null)
            return NotFound();

        _context.Items.Remove(item);
        _context.SaveChanges();

        return Ok("Item deleted successfully");
    }

    // ---------------- SUBSCRIPTION PLANS ----------------

    // View available plans
    [HttpGet("subscriptionplans")]
    public IActionResult GetSubscriptionPlans()
    {
        var plans = _context.SubscriptionPlans.ToList();
        return Ok(plans);
    }

    // Subscribe to a plan
   [HttpPost("{userId}/subscribe/{planId}")]
public IActionResult SubscribePlan(string userId, int planId)
{
    var subscription = new UserSubscription
    {
        UserId = userId,
        PlanId = planId,
        StartDate = DateTime.Now,
        ExpiryDate = DateTime.Now.AddMonths(1),
        SubscriptionType = "Monthly",
        PaymentStatus = "Paid"
    };

    _context.UserSubscriptions.Add(subscription);
    _context.SaveChanges();

    return Ok(subscription);
}

    // ---------------- TRANSACTIONS ----------------

    // Get transactions of a shop
    [HttpGet("{shopId}/transactions")]
    public IActionResult GetTransactions(int shopId)
    {
        var transactions = _context.Transactions
            .Where(t => t.ShopId == shopId)
            .ToList();

        return Ok(transactions);
    }
}