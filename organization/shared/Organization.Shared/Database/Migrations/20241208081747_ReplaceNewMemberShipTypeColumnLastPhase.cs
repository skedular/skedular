using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
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

            migrationBuilder.DropIndex(
                name: "IX_JoinInvitation_NewStatus",
                table: "JoinInvitation");

            migrationBuilder.DropColumn(
                name: "NewMembershipType",
                table: "OrganizationMember");

            migrationBuilder.DropColumn(
                name: "NewMembershipType",
                table: "JoinInvitation");

            migrationBuilder.DropColumn(
                name: "NewStatus",
                table: "JoinInvitation");

            migrationBuilder.AlterColumn<string>(
                name: "MembershipType",
                table: "OrganizationMember",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "JoinInvitation",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MembershipType",
                table: "JoinInvitation",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_MembershipType",
                table: "OrganizationMember",
                column: "MembershipType");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_Status",
                table: "JoinInvitation",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationMember_MembershipType",
                table: "OrganizationMember");

            migrationBuilder.DropIndex(
                name: "IX_JoinInvitation_Status",
                table: "JoinInvitation");

            migrationBuilder.AlterColumn<string>(
                name: "MembershipType",
                table: "OrganizationMember",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.AddColumn<string>(
                name: "NewMembershipType",
                table: "OrganizationMember",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "JoinInvitation",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<string>(
                name: "MembershipType",
                table: "JoinInvitation",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.AddColumn<string>(
                name: "NewMembershipType",
                table: "JoinInvitation",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewStatus",
                table: "JoinInvitation",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_NewMembershipType",
                table: "OrganizationMember",
                column: "NewMembershipType");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_NewStatus",
                table: "JoinInvitation",
                column: "NewStatus");
        }
    }
}
