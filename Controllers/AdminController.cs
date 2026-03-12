using Microsoft.AspNetCore.Mvc;
using ShopManagementAPI.Data;

namespace ShopManagementAPI.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("shops")]
    public IActionResult GetShops()
    {
        var shops = _context.Shops.ToList();
        return Ok(shops);
    }
}