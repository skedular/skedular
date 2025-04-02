using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexForCreatedAtAndUpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TermsOfUse_CreatedAt",
                table: "TermsOfUse",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TermsOfUse_ModifiedAt",
                table: "TermsOfUse",
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
                name: "IX_Tag_CreatedAt",
                table: "Tag",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_ModifiedAt",
                table: "Tag",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSsoSetting_CreatedAt",
                table: "OrganizationSsoSetting",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSsoSetting_ModifiedAt",
                table: "OrganizationSsoSetting",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOfferingActiveMember_CreatedAt",
                table: "OrganizationOfferingActiveMember",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOfferingActiveMember_ModifiedAt",
                table: "OrganizationOfferingActiveMember",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_CreatedAt",
                table: "OrganizationOffering",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_ModifiedAt",
                table: "OrganizationOffering",
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
                name: "IX_IndustrySubCategory_CreatedAt",
                table: "IndustrySubCategory",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_IndustrySubCategory_ModifiedAt",
                table: "IndustrySubCategory",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_IndustryMainCategory_CreatedAt",
                table: "IndustryMainCategory",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_IndustryMainCategory_ModifiedAt",
                table: "IndustryMainCategory",
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
                name: "IX_DailyMemberCountRecording_CreatedAt",
                table: "DailyMemberCountRecording",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DailyMemberCountRecording_ModifiedAt",
                table: "DailyMemberCountRecording",
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

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenantMember_CreatedAt",
                table: "AzureTenantMember",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenantMember_ModifiedAt",
                table: "AzureTenantMember",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenant_CreatedAt",
                table: "AzureTenant",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenant_ModifiedAt",
                table: "AzureTenant",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AzureInstallStateUserIdLookup_CreatedAt",
                table: "AzureInstallStateUserIdLookup",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AzureInstallStateUserIdLookup_ModifiedAt",
                table: "AzureInstallStateUserIdLookup",
                column: "ModifiedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TermsOfUse_CreatedAt",
                table: "TermsOfUse");

            migrationBuilder.DropIndex(
                name: "IX_TermsOfUse_ModifiedAt",
                table: "TermsOfUse");

            migrationBuilder.DropIndex(
                name: "IX_Team_CreatedAt",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Team_ModifiedAt",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Tag_CreatedAt",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Tag_ModifiedAt",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationSsoSetting_CreatedAt",
                table: "OrganizationSsoSetting");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationSsoSetting_ModifiedAt",
                table: "OrganizationSsoSetting");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationOfferingActiveMember_CreatedAt",
                table: "OrganizationOfferingActiveMember");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationOfferingActiveMember_ModifiedAt",
                table: "OrganizationOfferingActiveMember");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationOffering_CreatedAt",
                table: "OrganizationOffering");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationOffering_ModifiedAt",
                table: "OrganizationOffering");

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
                name: "IX_IndustrySubCategory_CreatedAt",
                table: "IndustrySubCategory");

            migrationBuilder.DropIndex(
                name: "IX_IndustrySubCategory_ModifiedAt",
                table: "IndustrySubCategory");

            migrationBuilder.DropIndex(
                name: "IX_IndustryMainCategory_CreatedAt",
                table: "IndustryMainCategory");

            migrationBuilder.DropIndex(
                name: "IX_IndustryMainCategory_ModifiedAt",
                table: "IndustryMainCategory");

            migrationBuilder.DropIndex(
                name: "IX_Identity_CreatedAt",
                table: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_Identity_ModifiedAt",
                table: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_DailyMemberCountRecording_CreatedAt",
                table: "DailyMemberCountRecording");

            migrationBuilder.DropIndex(
                name: "IX_DailyMemberCountRecording_ModifiedAt",
                table: "DailyMemberCountRecording");

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

            migrationBuilder.DropIndex(
                name: "IX_AzureTenantMember_CreatedAt",
                table: "AzureTenantMember");

            migrationBuilder.DropIndex(
                name: "IX_AzureTenantMember_ModifiedAt",
                table: "AzureTenantMember");

            migrationBuilder.DropIndex(
                name: "IX_AzureTenant_CreatedAt",
                table: "AzureTenant");

            migrationBuilder.DropIndex(
                name: "IX_AzureTenant_ModifiedAt",
                table: "AzureTenant");

            migrationBuilder.DropIndex(
                name: "IX_AzureInstallStateUserIdLookup_CreatedAt",
                table: "AzureInstallStateUserIdLookup");

            migrationBuilder.DropIndex(
                name: "IX_AzureInstallStateUserIdLookup_ModifiedAt",
                table: "AzureInstallStateUserIdLookup");
        }
    }
}
