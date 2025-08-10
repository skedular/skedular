using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueAlphaNumericNameToOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UniqueAlphanumericName",
                table: "Organization",
                type: "character varying(63)",
                maxLength: 63,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organization_UniqueAlphanumericName",
                table: "Organization",
                column: "UniqueAlphanumericName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organization_UniqueAlphanumericName",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "UniqueAlphanumericName",
                table: "Organization");
        }
    }
}
