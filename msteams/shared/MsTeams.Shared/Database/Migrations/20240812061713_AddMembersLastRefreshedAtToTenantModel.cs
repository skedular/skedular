using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsTeams.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMembersLastRefreshedAtToTenantModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EntitiesLastRefreshedAt",
                table: "Tenant",
                newName: "MembersLastRefreshedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MembersLastRefreshedAt",
                table: "Tenant",
                newName: "EntitiesLastRefreshedAt");
        }
    }
}
