using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamAndLocationMembersConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMember_Team_TeamId",
                table: "TeamMember");

            migrationBuilder.DropIndex(
                name: "IX_TeamMember_CustomerId",
                table: "TeamMember");

            migrationBuilder.DropIndex(
                name: "IX_LocationMember_CustomerId",
                table: "LocationMember");

            migrationBuilder.AlterColumn<string>(
                name: "TeamId",
                table: "TeamMember",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_CustomerId_TeamId",
                table: "TeamMember",
                columns: new[] { "CustomerId", "TeamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_CustomerId_LocationId",
                table: "LocationMember",
                columns: new[] { "CustomerId", "LocationId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMember_Team_TeamId",
                table: "TeamMember",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMember_Team_TeamId",
                table: "TeamMember");

            migrationBuilder.DropIndex(
                name: "IX_TeamMember_CustomerId_TeamId",
                table: "TeamMember");

            migrationBuilder.DropIndex(
                name: "IX_LocationMember_CustomerId_LocationId",
                table: "LocationMember");

            migrationBuilder.AlterColumn<string>(
                name: "TeamId",
                table: "TeamMember",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_CustomerId",
                table: "TeamMember",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_CustomerId",
                table: "LocationMember",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMember_Team_TeamId",
                table: "TeamMember",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id");
        }
    }
}
