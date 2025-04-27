using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOldBookingDataModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Team_TeamId",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_TeamId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Booking");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeamId",
                table: "Booking",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_TeamId",
                table: "Booking",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Team_TeamId",
                table: "Booking",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
