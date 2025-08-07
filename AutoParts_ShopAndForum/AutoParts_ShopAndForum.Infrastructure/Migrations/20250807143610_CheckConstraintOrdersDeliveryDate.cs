using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoParts_ShopAndForum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CheckConstraintOrdersDeliveryDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CC_Orders_DeliveryDate",
                table: "Orders",
                sql: "([IsDelivered] = 0 OR [DateDelivered] IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CC_Orders_DeliveryDate",
                table: "Orders");
        }
    }
}
