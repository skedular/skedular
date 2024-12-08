using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsTeams.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveComputedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NewMembershipType",
                table: "OrganizationMember",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true,
                oldComputedColumnSql: "\n                    CASE \n                        WHEN \"MembershipType\" = 0 THEN 'OWNER'\n                        WHEN \"MembershipType\" = 1 THEN 'ADMINISTRATOR'\n                        WHEN \"MembershipType\" = 2 THEN 'MEMBER'\n                        ELSE 'UNKNOWN'\n                    END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NewMembershipType",
                table: "OrganizationMember",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                computedColumnSql: "\n                    CASE \n                        WHEN \"MembershipType\" = 0 THEN 'OWNER'\n                        WHEN \"MembershipType\" = 1 THEN 'ADMINISTRATOR'\n                        WHEN \"MembershipType\" = 2 THEN 'MEMBER'\n                        ELSE 'UNKNOWN'\n                    END",
                stored: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);
        }
    }
}
