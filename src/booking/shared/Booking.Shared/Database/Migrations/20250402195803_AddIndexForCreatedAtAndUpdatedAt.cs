using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
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
                name: "IX_ResourceBookingSlot_CreatedAt",
                table: "ResourceBookingSlot",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceBookingSlot_ModifiedAt",
                table: "ResourceBookingSlot",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_CreatedAt",
                table: "Resource",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_ModifiedAt",
                table: "Resource",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTag_CreatedAt",
                table: "OrganizationTag",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTag_ModifiedAt",
                table: "OrganizationTag",
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
                name: "IX_LocationMember_CreatedAt",
                table: "LocationMember",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_ModifiedAt",
                table: "LocationMember",
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
                name: "IX_ResourceBookingSlot_CreatedAt",
                table: "ResourceBookingSlot");

            migrationBuilder.DropIndex(
                name: "IX_ResourceBookingSlot_ModifiedAt",
                table: "ResourceBookingSlot");

            migrationBuilder.DropIndex(
                name: "IX_Resource_CreatedAt",
                table: "Resource");

            migrationBuilder.DropIndex(
                name: "IX_Resource_ModifiedAt",
                table: "Resource");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationTag_CreatedAt",
                table: "OrganizationTag");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationTag_ModifiedAt",
                table: "OrganizationTag");

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
                name: "IX_LocationMember_CreatedAt",
                table: "LocationMember");

            migrationBuilder.DropIndex(
                name: "IX_LocationMember_ModifiedAt",
                table: "LocationMember");

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
                name: "IX_Booking_CreatedAt",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_ModifiedAt",
                table: "Booking");
        }
    }
}
