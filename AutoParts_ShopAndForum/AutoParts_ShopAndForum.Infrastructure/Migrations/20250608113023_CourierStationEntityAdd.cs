using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoParts_ShopAndForum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CourierStationEntityAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressType",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "CourierStationId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryStreet",
                table: "Orders",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CourierStation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FullAddress = table.Column<string>(type: "nvarchar(max)", maxLength: 1048576, nullable: true),
                    TownId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Firm = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourierStation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourierStation_Towns_TownId",
                        column: x => x.TownId,
                        principalTable: "Towns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CourierStationId",
                table: "Orders",
                column: "CourierStationId");

            migrationBuilder.CreateIndex(
                name: "IX_CourierStation_TownId",
                table: "CourierStation",
                column: "TownId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_CourierStation_CourierStationId",
                table: "Orders",
                column: "CourierStationId",
                principalTable: "CourierStation",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_CourierStation_CourierStationId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "CourierStation");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CourierStationId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CourierStationId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryStreet",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "AddressType",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Orders",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }
    }
}
