using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceNewMemberShipTypeColumnPhase1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeamMember_MembershipType",
                table: "TeamMember");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationMember_MembershipType",
                table: "OrganizationMember");

            migrationBuilder.DropIndex(
                name: "IX_JoinInvitation_Status",
                table: "JoinInvitation");

            migrationBuilder.AlterColumn<string>(
                name: "MembershipType",
                table: "TeamMember",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "MembershipType",
                table: "OrganizationMember",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "JoinInvitation",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "NewStatus",
                table: "JoinInvitation",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MembershipType",
                table: "JoinInvitation",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_NewMembershipType",
                table: "TeamMember",
                column: "NewMembershipType");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_NewMembershipType",
                table: "OrganizationMember",
                column: "NewMembershipType");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_NewStatus",
                table: "JoinInvitation",
                column: "NewStatus");

            migrationBuilder.Sql(@"UPDATE public.""JoinInvitation"" SET ""Status""= ""NewStatus""");
            migrationBuilder.Sql(@"UPDATE public.""OrganizationMember"" SET ""MembershipType""= ""NewMembershipType""");
            migrationBuilder.Sql(@"UPDATE public.""TeamMember"" SET ""MembershipType""= ""NewMembershipType""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeamMember_NewMembershipType",
                table: "TeamMember");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationMember_NewMembershipType",
                table: "OrganizationMember");

            migrationBuilder.DropIndex(
                name: "IX_JoinInvitation_NewStatus",
                table: "JoinInvitation");

            migrationBuilder.AlterColumn<int>(
                name: "MembershipType",
                table: "TeamMember",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MembershipType",
                table: "OrganizationMember",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "JoinInvitation",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NewStatus",
                table: "JoinInvitation",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MembershipType",
                table: "JoinInvitation",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_MembershipType",
                table: "TeamMember",
                column: "MembershipType");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_MembershipType",
                table: "OrganizationMember",
                column: "MembershipType");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_Status",
                table: "JoinInvitation",
                column: "Status");
        }
    }
}
