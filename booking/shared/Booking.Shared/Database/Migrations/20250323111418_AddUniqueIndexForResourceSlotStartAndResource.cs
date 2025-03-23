using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexForResourceSlotStartAndResource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ResourceBookingSlot_Start_ResourceId",
                table: "ResourceBookingSlot",
                columns: new[] { "Start", "ResourceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ResourceBookingSlot_Start_ResourceId",
                table: "ResourceBookingSlot");
        }
    }
}
