using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // 🔥 ADD THIS
using ShopManagementAPI.Data;
using ShopManagementAPI.Models;

namespace ShopManagementAPI.Controllers;

[Authorize] // 🔐 ADD THIS
[ApiController]
[Route("api/shops")]
public class ShopsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ShopsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var shops = _context.Shops.ToList();
        return Ok(shops);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var shop = _context.Shops.Find(id);
        if (shop == null)
            return NotFound("Shop not found");

        return Ok(shop);
    }

    [HttpPost]
    public IActionResult Create(Shop shop)
    {
        _context.Shops.Add(shop);
        _context.SaveChanges();
        return Ok(shop);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Shop updatedShop)
    {
        var shop = _context.Shops.Find(id);
        if (shop == null)
            return NotFound("Shop not found");

        shop.ShopName = updatedShop.ShopName;
        shop.ShopAddress = updatedShop.ShopAddress;
        shop.ContactNumber = updatedShop.ContactNumber;

        _context.SaveChanges();
        return Ok(shop);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var shop = _context.Shops.Find(id);
        if (shop == null)
            return NotFound("Shop not found");

        _context.Shops.Remove(shop);
        _context.SaveChanges();
        return Ok("Shop deleted successfully");
    }
}