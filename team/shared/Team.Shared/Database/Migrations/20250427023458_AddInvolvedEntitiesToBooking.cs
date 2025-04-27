using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddInvolvedEntitiesToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingTeam",
                columns: table => new
                {
                    InvolvedBookingsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    InvolvedTeamsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingTeam", x => new { x.InvolvedBookingsId, x.InvolvedTeamsId });
                    table.ForeignKey(
                        name: "FK_BookingTeam_Booking_InvolvedBookingsId",
                        column: x => x.InvolvedBookingsId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingTeam_Team_InvolvedTeamsId",
                        column: x => x.InvolvedTeamsId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingTeam_InvolvedTeamsId",
                table: "BookingTeam",
                column: "InvolvedTeamsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingTeam");
        }
    }
}
