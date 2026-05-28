using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsTeams.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexForCreatedAtAndUpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Team_CreatedAt",
                table: "Team",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Team_ModifiedAt",
                table: "Team",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_CreatedAt",
                table: "OrganizationMember",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_ModifiedAt",
                table: "OrganizationMember",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_CreatedAt",
                table: "Organization",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_ModifiedAt",
                table: "Organization",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Location_CreatedAt",
                table: "Location",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Location_ModifiedAt",
                table: "Location",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_CreatedAt",
                table: "Identity",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_ModifiedAt",
                table: "Identity",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CreatedAt",
                table: "Customer",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_ModifiedAt",
                table: "Customer",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenantTeamChannel_CreatedAt",
                table: "AzureTenantTeamChannel",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenantTeamChannel_ModifiedAt",
                table: "AzureTenantTeamChannel",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenantTeam_CreatedAt",
                table: "AzureTenantTeam",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenantTeam_ModifiedAt",
                table: "AzureTenantTeam",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenant_CreatedAt",
                table: "AzureTenant",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenant_ModifiedAt",
                table: "AzureTenant",
                column: "ModifiedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Team_CreatedAt",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Team_ModifiedAt",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationMember_CreatedAt",
                table: "OrganizationMember");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationMember_ModifiedAt",
                table: "OrganizationMember");

            migrationBuilder.DropIndex(
                name: "IX_Organization_CreatedAt",
                table: "Organization");

            migrationBuilder.DropIndex(
                name: "IX_Organization_ModifiedAt",
                table: "Organization");

            migrationBuilder.DropIndex(
                name: "IX_Location_CreatedAt",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Location_ModifiedAt",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Identity_CreatedAt",
                table: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_Identity_ModifiedAt",
                table: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_Customer_CreatedAt",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Customer_ModifiedAt",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_AzureTenantTeamChannel_CreatedAt",
                table: "AzureTenantTeamChannel");

            migrationBuilder.DropIndex(
                name: "IX_AzureTenantTeamChannel_ModifiedAt",
                table: "AzureTenantTeamChannel");

            migrationBuilder.DropIndex(
                name: "IX_AzureTenantTeam_CreatedAt",
                table: "AzureTenantTeam");

            migrationBuilder.DropIndex(
                name: "IX_AzureTenantTeam_ModifiedAt",
                table: "AzureTenantTeam");

            migrationBuilder.DropIndex(
                name: "IX_AzureTenant_CreatedAt",
                table: "AzureTenant");

            migrationBuilder.DropIndex(
                name: "IX_AzureTenant_ModifiedAt",
                table: "AzureTenant");
        }
    }
}
