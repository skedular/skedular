using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MadeOldMembershipType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewMembershipType",
                table: "OrganizationMember",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                computedColumnSql: "\n                    CASE \n                        WHEN \"MembershipType\" = 0 THEN 'OWNER'\n                        WHEN \"MembershipType\" = 1 THEN 'ADMINISTRATOR'\n                        WHEN \"MembershipType\" = 2 THEN 'MEMBER'\n                        ELSE 'UNKNOWN'\n                    END",
                stored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewMembershipType",
                table: "OrganizationMember");
        }
    }
}
