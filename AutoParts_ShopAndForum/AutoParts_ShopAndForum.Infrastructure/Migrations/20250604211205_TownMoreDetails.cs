using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoParts_ShopAndForum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TownMoreDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCity",
                table: "Towns",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostCode",
                table: "Towns",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCity",
                table: "Towns");

            migrationBuilder.DropColumn(
                name: "PostCode",
                table: "Towns");
        }
    }
}
