using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notification.Shared.Database.Migrations
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
                name: "IX_Organization_CreatedAt",
                table: "Organization",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_ModifiedAt",
                table: "Organization",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_CreatedAt",
                table: "Notification",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ModifiedAt",
                table: "Notification",
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
                name: "IX_Team_CreatedAt",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Team_ModifiedAt",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Organization_CreatedAt",
                table: "Organization");

            migrationBuilder.DropIndex(
                name: "IX_Organization_ModifiedAt",
                table: "Organization");

            migrationBuilder.DropIndex(
                name: "IX_Notification_CreatedAt",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_ModifiedAt",
                table: "Notification");

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
