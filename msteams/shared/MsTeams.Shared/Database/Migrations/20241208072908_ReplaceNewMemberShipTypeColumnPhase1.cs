using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsTeams.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceNewMemberShipTypeColumnPhase1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationMember_MembershipType",
                table: "OrganizationMember");

            migrationBuilder.AlterColumn<string>(
                name: "MembershipType",
                table: "OrganizationMember",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_NewMembershipType",
                table: "OrganizationMember",
                column: "NewMembershipType");

            migrationBuilder.Sql(@"UPDATE public.""OrganizationMember"" SET ""MembershipType""= ""NewMembershipType""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationMember_NewMembershipType",
                table: "OrganizationMember");

            migrationBuilder.AlterColumn<int>(
                name: "MembershipType",
                table: "OrganizationMember",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_MembershipType",
                table: "OrganizationMember",
                column: "MembershipType");
        }
    }
}
