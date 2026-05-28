using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReplacedUniqueAlphanumericNameWithCustomDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UniqueAlphanumericName",
                table: "Organization",
                newName: "CustomDomain");

            migrationBuilder.RenameIndex(
                name: "IX_Organization_UniqueAlphanumericName",
                table: "Organization",
                newName: "IX_Organization_CustomDomain");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomDomain",
                table: "Organization",
                newName: "UniqueAlphanumericName");

            migrationBuilder.RenameIndex(
                name: "IX_Organization_CustomDomain",
                table: "Organization",
                newName: "IX_Organization_UniqueAlphanumericName");
        }
    }
}
