using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceNewMemberShipTypeColumnPhase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MembershipType",
                table: "TeamMember",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.Sql(@"UPDATE public.""OrganizationMember"" SET ""MembershipType""= ""NewMembershipType""");
            migrationBuilder.Sql(@"UPDATE public.""LocationMember"" SET ""MembershipType""= ""NewMembershipType""");
            migrationBuilder.Sql(@"UPDATE public.""TeamMember"" SET ""MembershipType""= ""NewMembershipType""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MembershipType",
                table: "TeamMember");
        }
    }
}
