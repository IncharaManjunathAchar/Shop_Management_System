using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopManagementAPI.Migrations
{
    public partial class UpdatePlanMaxShops : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Free Trial → 1 shop only
            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 1,
                column: "MaxShops",
                value: 1);

            // Monthly → 5 shops
            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 2,
                column: "MaxShops",
                value: 5);

            // Yearly → 7 shops
            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 3,
                column: "MaxShops",
                value: 7);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(table: "SubscriptionPlans", keyColumn: "PlanId", keyValue: 1, column: "MaxShops", value: 0);
            migrationBuilder.UpdateData(table: "SubscriptionPlans", keyColumn: "PlanId", keyValue: 2, column: "MaxShops", value: 0);
            migrationBuilder.UpdateData(table: "SubscriptionPlans", keyColumn: "PlanId", keyValue: 3, column: "MaxShops", value: 0);
        }
    }
}
