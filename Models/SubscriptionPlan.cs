using System.ComponentModel.DataAnnotations;

namespace ShopManagementAPI.Models;

public class SubscriptionPlan
{
    [Key]
    public int PlanId { get; set; }

    public required string PlanName { get; set; }

    public int DurationDays { get; set; }

    public decimal Price { get; set; }

    public int TrialDays { get; set; }

    public required string Description { get; set; }
}