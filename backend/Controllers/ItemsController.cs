using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShopManagementAPI.Data;
using ShopManagementAPI.Models;
using System.Security.Claims;

namespace ShopManagementAPI.Controllers;

[Authorize(Roles = "Shopkeeper")]
[ApiController]
[Route("api/items")]
public class ItemsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ItemsController(AppDbContext context)
    {
        _context = context;
    }

    private bool OwnsShop(int shopId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return _context.Shops.Any(s => s.ShopId == shopId && s.UserId == userId);
    }

    [HttpGet("shop/{shopId}")]
    public IActionResult GetByShop(int shopId)
    {
        if (!OwnsShop(shopId)) return Forbid();
        var items = _context.Items.Where(i => i.ShopId == shopId).ToList();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var item = _context.Items.FirstOrDefault(i => i.ItemId == id);
        if (item == null) return NotFound("Item not found");
        if (!OwnsShop(item.ShopId)) return Forbid();
        return Ok(item);
    }

    [HttpPost]
    public IActionResult Create(Item item)
    {
        if (!OwnsShop(item.ShopId)) return Forbid();
        _context.Items.Add(item);
        _context.SaveChanges();
        return Ok(item);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Item updatedItem)
    {
        var item = _context.Items.FirstOrDefault(i => i.ItemId == id);
        if (item == null) return NotFound("Item not found");
        if (!OwnsShop(item.ShopId)) return Forbid();

        item.ItemName = updatedItem.ItemName;
        item.Quantity = updatedItem.Quantity;
        item.CostPrice = updatedItem.CostPrice;
        item.SellingPrice = updatedItem.SellingPrice;
        item.ExpiryDate = updatedItem.ExpiryDate;

        _context.SaveChanges();
        return Ok(item);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var item = _context.Items.FirstOrDefault(i => i.ItemId == id);
        if (item == null) return NotFound("Item not found");
        if (!OwnsShop(item.ShopId)) return Forbid();

        _context.Items.Remove(item);
        _context.SaveChanges();
        return Ok("Item deleted successfully");
    }

    [HttpGet("shop/{shopId}/expiring")]
    public IActionResult GetExpiring(int shopId, [FromQuery] int days = 30)
    {
        if (!OwnsShop(shopId)) return Forbid();
        var threshold = DateTime.Now.AddDays(days);
        var items = _context.Items
            .Where(i => i.ShopId == shopId &&
                        i.ExpiryDate <= threshold &&
                        i.ExpiryDate >= DateTime.Now)
            .ToList();
        return Ok(items);
    }
}