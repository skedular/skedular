using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceNewMemberShipTypeColumnLastPhase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationMember_NewMembershipType",
                table: "OrganizationMember");

            migrationBuilder.DropColumn(
                name: "NewMembershipType",
                table: "OrganizationMember");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_MembershipType",
                table: "OrganizationMember",
                column: "MembershipType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationMember_MembershipType",
                table: "OrganizationMember");

            migrationBuilder.AddColumn<string>(
                name: "NewMembershipType",
                table: "OrganizationMember",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_NewMembershipType",
                table: "OrganizationMember",
                column: "NewMembershipType");
        }
    }
}
