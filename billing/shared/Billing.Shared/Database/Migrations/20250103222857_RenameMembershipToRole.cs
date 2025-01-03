using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameMembershipToRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MembershipType",
                table: "OrganizationMember",
                newName: "Role");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationMember_MembershipType",
                table: "OrganizationMember",
                newName: "IX_OrganizationMember_Role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "OrganizationMember",
                newName: "MembershipType");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationMember_Role",
                table: "OrganizationMember",
                newName: "IX_OrganizationMember_MembershipType");
        }
    }
}
