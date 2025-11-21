using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Migrations
{
    /// <inheritdoc />
    public partial class APrecomputedResourceListOBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingResource",
                columns: table => new
                {
                    InvolvedBookingsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    InvolvedResourcesId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingResource", x => new { x.InvolvedBookingsId, x.InvolvedResourcesId });
                    table.ForeignKey(
                        name: "FK_BookingResource_Booking_InvolvedBookingsId",
                        column: x => x.InvolvedBookingsId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingResource_Resource_InvolvedResourcesId",
                        column: x => x.InvolvedResourcesId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingResource_InvolvedResourcesId",
                table: "BookingResource",
                column: "InvolvedResourcesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingResource");
        }
    }
}
