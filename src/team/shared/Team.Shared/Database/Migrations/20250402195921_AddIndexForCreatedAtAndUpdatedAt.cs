using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexForCreatedAtAndUpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_CreatedAt",
                table: "TeamMember",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_ModifiedAt",
                table: "TeamMember",
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
                name: "IX_JoinInvitation_CreatedAt",
                table: "JoinInvitation",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_ModifiedAt",
                table: "JoinInvitation",
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
                name: "IX_Booking_CreatedAt",
                table: "Booking",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_ModifiedAt",
                table: "Booking",
                column: "ModifiedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeamMember_CreatedAt",
                table: "TeamMember");

            migrationBuilder.DropIndex(
                name: "IX_TeamMember_ModifiedAt",
                table: "TeamMember");

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
                name: "IX_JoinInvitation_CreatedAt",
                table: "JoinInvitation");

            migrationBuilder.DropIndex(
                name: "IX_JoinInvitation_ModifiedAt",
                table: "JoinInvitation");

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
                name: "IX_Booking_CreatedAt",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_ModifiedAt",
                table: "Booking");
        }
    }
}
