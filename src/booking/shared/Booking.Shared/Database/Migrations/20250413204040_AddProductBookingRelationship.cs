using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddProductBookingRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<BookingSchedules>(
                name: "BookingSchedules",
                table: "Booking",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BookingProductVersion",
                columns: table => new
                {
                    BookingsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ProductVersionsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingProductVersion", x => new { x.BookingsId, x.ProductVersionsId });
                    table.ForeignKey(
                        name: "FK_BookingProductVersion_Booking_BookingsId",
                        column: x => x.BookingsId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingProductVersion_ProductVersion_ProductVersionsId",
                        column: x => x.ProductVersionsId,
                        principalTable: "ProductVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingProductVersion_ProductVersionsId",
                table: "BookingProductVersion",
                column: "ProductVersionsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingProductVersion");

            migrationBuilder.DropColumn(
                name: "BookingSchedules",
                table: "Booking");
        }
    }
}
