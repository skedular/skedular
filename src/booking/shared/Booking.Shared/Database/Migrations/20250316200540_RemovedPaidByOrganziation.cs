using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemovedPaidByOrganziation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Customer_CreatedByCustomerId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Customer_PaidByCustomerId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Organization_PaidByOrganizationId",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_CreatedByCustomerId",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_PaidByCustomerId",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_PaidByOrganizationId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "CreatedByCustomerId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "PaidByCustomerId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "PaidByOrganizationId",
                table: "Booking");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByCustomerId",
                table: "Booking",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaidByCustomerId",
                table: "Booking",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaidByOrganizationId",
                table: "Booking",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_CreatedByCustomerId",
                table: "Booking",
                column: "CreatedByCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_PaidByCustomerId",
                table: "Booking",
                column: "PaidByCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_PaidByOrganizationId",
                table: "Booking",
                column: "PaidByOrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Customer_CreatedByCustomerId",
                table: "Booking",
                column: "CreatedByCustomerId",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Customer_PaidByCustomerId",
                table: "Booking",
                column: "PaidByCustomerId",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Organization_PaidByOrganizationId",
                table: "Booking",
                column: "PaidByOrganizationId",
                principalTable: "Organization",
                principalColumn: "Id");
        }
    }
}
