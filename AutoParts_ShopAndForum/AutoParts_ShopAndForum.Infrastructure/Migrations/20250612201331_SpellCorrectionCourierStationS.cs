using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoParts_ShopAndForum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SpellCorrectionCourierStationS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourierStation_Towns_TownId",
                table: "CourierStation");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_CourierStation_CourierStationId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourierStation",
                table: "CourierStation");

            migrationBuilder.RenameTable(
                name: "CourierStation",
                newName: "CourierStations");

            migrationBuilder.RenameIndex(
                name: "IX_CourierStation_TownId",
                table: "CourierStations",
                newName: "IX_CourierStations_TownId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourierStations",
                table: "CourierStations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourierStations_Towns_TownId",
                table: "CourierStations",
                column: "TownId",
                principalTable: "Towns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_CourierStations_CourierStationId",
                table: "Orders",
                column: "CourierStationId",
                principalTable: "CourierStations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourierStations_Towns_TownId",
                table: "CourierStations");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_CourierStations_CourierStationId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourierStations",
                table: "CourierStations");

            migrationBuilder.RenameTable(
                name: "CourierStations",
                newName: "CourierStation");

            migrationBuilder.RenameIndex(
                name: "IX_CourierStations_TownId",
                table: "CourierStation",
                newName: "IX_CourierStation_TownId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourierStation",
                table: "CourierStation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourierStation_Towns_TownId",
                table: "CourierStation",
                column: "TownId",
                principalTable: "Towns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_CourierStation_CourierStationId",
                table: "Orders",
                column: "CourierStationId",
                principalTable: "CourierStation",
                principalColumn: "Id");
        }
    }
}
