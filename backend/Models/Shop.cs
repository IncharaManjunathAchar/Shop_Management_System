namespace ShopManagementAPI.Models;

public class Shop
{
    public int ShopId { get; set; }

    public required string UserId { get; set; }

    public required string ShopName { get; set; }

    public required string ShopAddress { get; set; }

    public required string ContactNumber { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}