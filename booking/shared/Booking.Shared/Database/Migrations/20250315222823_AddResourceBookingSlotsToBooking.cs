using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddResourceBookingSlotsToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingResourceBookingSlot",
                columns: table => new
                {
                    BookingsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ResourceBookingSlotsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingResourceBookingSlot", x => new { x.BookingsId, x.ResourceBookingSlotsId });
                    table.ForeignKey(
                        name: "FK_BookingResourceBookingSlot_Booking_BookingsId",
                        column: x => x.BookingsId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingResourceBookingSlot_ResourceBookingSlot_ResourceBook~",
                        column: x => x.ResourceBookingSlotsId,
                        principalTable: "ResourceBookingSlot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingResourceBookingSlot_ResourceBookingSlotsId",
                table: "BookingResourceBookingSlot",
                column: "ResourceBookingSlotsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingResourceBookingSlot");
        }
    }
}
