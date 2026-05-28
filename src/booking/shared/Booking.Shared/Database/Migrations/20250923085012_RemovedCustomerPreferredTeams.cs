using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Migrations
{
    /// <inheritdoc />
    public partial class RemovedCustomerPreferredTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerTeam");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerTeam",
                columns: table => new
                {
                    PreferredByCustomersId = table.Column<string>(type: "character varying(100)", nullable: false),
                    PreferredTeamsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerTeam", x => new { x.PreferredByCustomersId, x.PreferredTeamsId });
                    table.ForeignKey(
                        name: "FK_CustomerTeam_Customer_PreferredByCustomersId",
                        column: x => x.PreferredByCustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerTeam_Team_PreferredTeamsId",
                        column: x => x.PreferredTeamsId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTeam_PreferredTeamsId",
                table: "CustomerTeam",
                column: "PreferredTeamsId");
        }
    }
}
