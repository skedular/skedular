using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSpacesTrialState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SpacesBillingStartsAt",
                table: "OrganizationOffering",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SpacesTrialStartedAt",
                table: "Organization",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_SpacesBillingStartsAt",
                table: "OrganizationOffering",
                column: "SpacesBillingStartsAt");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_SpacesTrialStartedAt",
                table: "Organization",
                column: "SpacesTrialStartedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationOffering_SpacesBillingStartsAt",
                table: "OrganizationOffering");

            migrationBuilder.DropIndex(
                name: "IX_Organization_SpacesTrialStartedAt",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "SpacesBillingStartsAt",
                table: "OrganizationOffering");

            migrationBuilder.DropColumn(
                name: "SpacesTrialStartedAt",
                table: "Organization");
        }
    }
}
