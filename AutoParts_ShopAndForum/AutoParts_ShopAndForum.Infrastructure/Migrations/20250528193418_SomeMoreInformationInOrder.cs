using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoParts_ShopAndForum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SomeMoreInformationInOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AddressType",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InvoicePersonFirstName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InvoicePersonLastName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressType",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "InvoicePersonFirstName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "InvoicePersonLastName",
                table: "Orders");
        }
    }
}
