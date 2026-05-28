using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOldBookingDataModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Customer_CustomerId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Location_LocationId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Organization_OrganizationId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Team_TeamId",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_CustomerId",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_LocationId",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_OrganizationId",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_TeamId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Booking");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "Booking",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LocationId",
                table: "Booking",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "Booking",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeamId",
                table: "Booking",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_CustomerId",
                table: "Booking",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_LocationId",
                table: "Booking",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_OrganizationId",
                table: "Booking",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_TeamId",
                table: "Booking",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Customer_CustomerId",
                table: "Booking",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Location_LocationId",
                table: "Booking",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Organization_OrganizationId",
                table: "Booking",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Team_TeamId",
                table: "Booking",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id");
        }
    }
}
