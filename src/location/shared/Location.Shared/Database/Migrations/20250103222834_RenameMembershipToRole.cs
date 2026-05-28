using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
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

            migrationBuilder.RenameColumn(
                name: "MembershipType",
                table: "LocationMember",
                newName: "Role");

            migrationBuilder.RenameIndex(
                name: "IX_LocationMember_MembershipType",
                table: "LocationMember",
                newName: "IX_LocationMember_Role");

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
                table: "OrganizationMember",
                newName: "MembershipType");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationMember_Role",
                table: "OrganizationMember",
                newName: "IX_OrganizationMember_MembershipType");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "LocationMember",
                newName: "MembershipType");

            migrationBuilder.RenameIndex(
                name: "IX_LocationMember_Role",
                table: "LocationMember",
                newName: "IX_LocationMember_MembershipType");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "JoinInvitation",
                newName: "MembershipType");
        }
    }
}
