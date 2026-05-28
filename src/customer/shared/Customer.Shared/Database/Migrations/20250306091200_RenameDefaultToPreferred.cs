using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameDefaultToPreferred : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerLocation_Customer_DefaultedByCustomersId",
                table: "CustomerLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerLocation_Location_DefaultLocationsId",
                table: "CustomerLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTeam_Customer_DefaultedByCustomersId",
                table: "CustomerTeam");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTeam_Team_DefaultTeamsId",
                table: "CustomerTeam");

            migrationBuilder.RenameColumn(
                name: "DefaultedByCustomersId",
                table: "CustomerTeam",
                newName: "PreferredTeamsId");

            migrationBuilder.RenameColumn(
                name: "DefaultTeamsId",
                table: "CustomerTeam",
                newName: "PreferredByCustomersId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerTeam_DefaultedByCustomersId",
                table: "CustomerTeam",
                newName: "IX_CustomerTeam_PreferredTeamsId");

            migrationBuilder.RenameColumn(
                name: "DefaultedByCustomersId",
                table: "CustomerLocation",
                newName: "PreferredLocationsId");

            migrationBuilder.RenameColumn(
                name: "DefaultLocationsId",
                table: "CustomerLocation",
                newName: "PreferredByCustomersId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerLocation_DefaultedByCustomersId",
                table: "CustomerLocation",
                newName: "IX_CustomerLocation_PreferredLocationsId");

            migrationBuilder.RenameColumn(
                name: "IsDefaultLocationOnboardingDone",
                table: "Customer",
                newName: "IsPreferredLocationOnboardingDone");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerLocation_Customer_PreferredByCustomersId",
                table: "CustomerLocation",
                column: "PreferredByCustomersId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerLocation_Location_PreferredLocationsId",
                table: "CustomerLocation",
                column: "PreferredLocationsId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTeam_Customer_PreferredByCustomersId",
                table: "CustomerTeam",
                column: "PreferredByCustomersId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTeam_Team_PreferredTeamsId",
                table: "CustomerTeam",
                column: "PreferredTeamsId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerLocation_Customer_PreferredByCustomersId",
                table: "CustomerLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerLocation_Location_PreferredLocationsId",
                table: "CustomerLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTeam_Customer_PreferredByCustomersId",
                table: "CustomerTeam");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTeam_Team_PreferredTeamsId",
                table: "CustomerTeam");

            migrationBuilder.RenameColumn(
                name: "PreferredTeamsId",
                table: "CustomerTeam",
                newName: "DefaultedByCustomersId");

            migrationBuilder.RenameColumn(
                name: "PreferredByCustomersId",
                table: "CustomerTeam",
                newName: "DefaultTeamsId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerTeam_PreferredTeamsId",
                table: "CustomerTeam",
                newName: "IX_CustomerTeam_DefaultedByCustomersId");

            migrationBuilder.RenameColumn(
                name: "PreferredLocationsId",
                table: "CustomerLocation",
                newName: "DefaultedByCustomersId");

            migrationBuilder.RenameColumn(
                name: "PreferredByCustomersId",
                table: "CustomerLocation",
                newName: "DefaultLocationsId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerLocation_PreferredLocationsId",
                table: "CustomerLocation",
                newName: "IX_CustomerLocation_DefaultedByCustomersId");

            migrationBuilder.RenameColumn(
                name: "IsPreferredLocationOnboardingDone",
                table: "Customer",
                newName: "IsDefaultLocationOnboardingDone");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerLocation_Customer_DefaultedByCustomersId",
                table: "CustomerLocation",
                column: "DefaultedByCustomersId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerLocation_Location_DefaultLocationsId",
                table: "CustomerLocation",
                column: "DefaultLocationsId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTeam_Customer_DefaultedByCustomersId",
                table: "CustomerTeam",
                column: "DefaultedByCustomersId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTeam_Team_DefaultTeamsId",
                table: "CustomerTeam",
                column: "DefaultTeamsId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
