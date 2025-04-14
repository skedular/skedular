using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberVisibilityPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MemberVisibilityPolicy",
                table: "Organization",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "FULL_ACCESS");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Organization",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "PRIVATE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberVisibilityPolicy",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Organization");
        }
    }
}
