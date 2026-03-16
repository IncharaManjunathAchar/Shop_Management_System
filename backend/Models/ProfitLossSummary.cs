namespace ShopManagementAPI.Models;

public class ProfitLossSummary
{
    public int ShopId { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalCost { get; set; }
    public decimal Profit { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}
