using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoParts_ShopAndForum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedOnlinePaymentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OnlinePaymentStatus",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PublicToken",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnlinePaymentStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PublicToken",
                table: "Orders");
        }
    }
}
