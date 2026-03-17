using System.ComponentModel.DataAnnotations.Schema;

namespace ShopManagementAPI.Models;

public class Item
{
    public int ItemId { get; set; }

    public int ShopId { get; set; }

    [ForeignKey("ShopId")]
    public Shop? Shop { get; set; }

    public required string ItemName { get; set; }

    public int Quantity { get; set; }

    public decimal CostPrice { get; set; }

    public decimal SellingPrice { get; set; }

    public DateTime ExpiryDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}