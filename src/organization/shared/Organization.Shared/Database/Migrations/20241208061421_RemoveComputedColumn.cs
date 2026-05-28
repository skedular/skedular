using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
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
                oldComputedColumnSql: "\n                    CASE \n                        WHEN \"MembershipType\" = 0 THEN 'OWNER'\n                        WHEN \"MembershipType\" = 1 THEN 'ADMINISTRATOR'\n                        WHEN \"MembershipType\" = 2 THEN 'MEMBER'\n                        ELSE 'UNKNOWN'\n                    END");

            migrationBuilder.AlterColumn<string>(
                name: "NewStatus",
                table: "JoinInvitation",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "NewMembershipType",
                table: "JoinInvitation",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldComputedColumnSql: "\n                    CASE \n                        WHEN \"MembershipType\" = 0 THEN 'OWNER'\n                        WHEN \"MembershipType\" = 1 THEN 'ADMINISTRATOR'\n                        WHEN \"MembershipType\" = 2 THEN 'MEMBER'\n                        ELSE 'UNKNOWN'\n                    END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NewStatus",
                table: "JoinInvitation",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NewMembershipType",
                table: "OrganizationMember",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                computedColumnSql: "\n                    CASE \n                        WHEN \"MembershipType\" = 0 THEN 'OWNER'\n                        WHEN \"MembershipType\" = 1 THEN 'ADMINISTRATOR'\n                        WHEN \"MembershipType\" = 2 THEN 'MEMBER'\n                        ELSE 'UNKNOWN'\n                    END",
                stored: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NewMembershipType",
                table: "JoinInvitation",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                computedColumnSql: "\n                    CASE \n                        WHEN \"MembershipType\" = 0 THEN 'OWNER'\n                        WHEN \"MembershipType\" = 1 THEN 'ADMINISTRATOR'\n                        WHEN \"MembershipType\" = 2 THEN 'MEMBER'\n                        ELSE 'UNKNOWN'\n                    END",
                stored: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);
        }
    }
}
