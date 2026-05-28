using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slack.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexForCreatedAtAndUpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceMember_CreatedAt",
                table: "WorkspaceMember",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceMember_ModifiedAt",
                table: "WorkspaceMember",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceChannel_CreatedAt",
                table: "WorkspaceChannel",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceChannel_ModifiedAt",
                table: "WorkspaceChannel",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Workspace_CreatedAt",
                table: "Workspace",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Workspace_ModifiedAt",
                table: "Workspace",
                column: "ModifiedAt");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkspaceMember_CreatedAt",
                table: "WorkspaceMember");

            migrationBuilder.DropIndex(
                name: "IX_WorkspaceMember_ModifiedAt",
                table: "WorkspaceMember");

            migrationBuilder.DropIndex(
                name: "IX_WorkspaceChannel_CreatedAt",
                table: "WorkspaceChannel");

            migrationBuilder.DropIndex(
                name: "IX_WorkspaceChannel_ModifiedAt",
                table: "WorkspaceChannel");

            migrationBuilder.DropIndex(
                name: "IX_Workspace_CreatedAt",
                table: "Workspace");

            migrationBuilder.DropIndex(
                name: "IX_Workspace_ModifiedAt",
                table: "Workspace");

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
        }
    }
}
