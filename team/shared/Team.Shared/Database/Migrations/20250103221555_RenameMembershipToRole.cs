using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameMembershipToRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MembershipType",
                table: "TeamMember",
                newName: "Role");

            migrationBuilder.RenameIndex(
                name: "IX_TeamMember_MembershipType",
                table: "TeamMember",
                newName: "IX_TeamMember_Role");

            migrationBuilder.RenameColumn(
                name: "MembershipType",
                table: "OrganizationMember",
                newName: "Role");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationMember_MembershipType",
                table: "OrganizationMember",
                newName: "IX_OrganizationMember_Role");

            migrationBuilder.RenameColumn(
                name: "MembershipType",
                table: "JoinInvitation",
                newName: "Role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "TeamMember",
                newName: "MembershipType");

            migrationBuilder.RenameIndex(
                name: "IX_TeamMember_Role",
                table: "TeamMember",
                newName: "IX_TeamMember_MembershipType");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "OrganizationMember",
                newName: "MembershipType");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationMember_Role",
                table: "OrganizationMember",
                newName: "IX_OrganizationMember_MembershipType");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "JoinInvitation",
                newName: "MembershipType");
        }
    }
}
