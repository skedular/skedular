using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class SetMembershipTypeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE public.""OrganizationMember""
                                    SET ""NewMembershipType""= CASE 
                                        WHEN ""MembershipType"" = 0 THEN 'OWNER'
                                        WHEN ""MembershipType"" = 1 THEN 'ADMINISTRATOR'
                                        WHEN ""MembershipType"" = 2 THEN 'MEMBER'
                                        ELSE 'UNKNOWN'
                                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
