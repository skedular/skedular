using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceNewMemberShipTypeColumnLastPhase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeamMember_NewMembershipType",
                table: "TeamMember");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationMember_NewMembershipType",
                table: "OrganizationMember");

            migrationBuilder.DropIndex(
                name: "IX_LocationMember_NewMembershipType",
                table: "LocationMember");

            migrationBuilder.DropColumn(
                name: "NewMembershipType",
                table: "TeamMember");

            migrationBuilder.DropColumn(
                name: "NewMembershipType",
                table: "OrganizationMember");

            migrationBuilder.DropColumn(
                name: "NewMembershipType",
                table: "LocationMember");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_MembershipType",
                table: "TeamMember",
                column: "MembershipType");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_MembershipType",
                table: "OrganizationMember",
                column: "MembershipType");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_MembershipType",
                table: "LocationMember",
                column: "MembershipType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeamMember_MembershipType",
                table: "TeamMember");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationMember_MembershipType",
                table: "OrganizationMember");

            migrationBuilder.DropIndex(
                name: "IX_LocationMember_MembershipType",
                table: "LocationMember");

            migrationBuilder.AddColumn<string>(
                name: "NewMembershipType",
                table: "TeamMember",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewMembershipType",
                table: "OrganizationMember",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewMembershipType",
                table: "LocationMember",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_NewMembershipType",
                table: "TeamMember",
                column: "NewMembershipType");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_NewMembershipType",
                table: "OrganizationMember",
                column: "NewMembershipType");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_NewMembershipType",
                table: "LocationMember",
                column: "NewMembershipType");
        }
    }
}
