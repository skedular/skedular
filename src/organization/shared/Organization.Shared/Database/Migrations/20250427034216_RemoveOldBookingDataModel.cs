using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOldBookingDataModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Organization_OrganizationId",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_OrganizationId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Booking");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "Booking",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_OrganizationId",
                table: "Booking",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Organization_OrganizationId",
                table: "Booking",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
