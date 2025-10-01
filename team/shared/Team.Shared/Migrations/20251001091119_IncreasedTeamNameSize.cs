using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team.Shared.Migrations
{
    /// <inheritdoc />
    public partial class IncreasedTeamNameSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Location_Name",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Location");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Team",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Team",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Location",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Location_Name",
                table: "Location",
                column: "Name");
        }
    }
}
