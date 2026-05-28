using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddResourceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResourceType",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceType_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TermsOfUse_Active",
                table: "TermsOfUse",
                column: "Active");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_Description",
                table: "Tag",
                column: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_Name",
                table: "Tag",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_Type",
                table: "Tag",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_IsOrganizationOnboardingDone",
                table: "OrganizationMember",
                column: "IsOrganizationOnboardingDone");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_HasAttachedPaymentMethod",
                table: "Organization",
                column: "HasAttachedPaymentMethod");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_PaymentMethodEventRaisedAt",
                table: "Organization",
                column: "PaymentMethodEventRaisedAt");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_Role",
                table: "JoinInvitation",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_IndustrySubCategory_Name",
                table: "IndustrySubCategory",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_IndustryMainCategory_Name",
                table: "IndustryMainCategory",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenant_InstalledByUserId",
                table: "AzureTenant",
                column: "InstalledByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenant_MembersLastRefreshedAt",
                table: "AzureTenant",
                column: "MembersLastRefreshedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenant_Name",
                table: "AzureTenant",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceType_DeletedAt",
                table: "ResourceType",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceType_Description",
                table: "ResourceType",
                column: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceType_Name",
                table: "ResourceType",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceType_OrganizationId",
                table: "ResourceType",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceType_Type",
                table: "ResourceType",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceType");

            migrationBuilder.DropIndex(
                name: "IX_TermsOfUse_Active",
                table: "TermsOfUse");

            migrationBuilder.DropIndex(
                name: "IX_Tag_Description",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Tag_Name",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Tag_Type",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationMember_IsOrganizationOnboardingDone",
                table: "OrganizationMember");

            migrationBuilder.DropIndex(
                name: "IX_Organization_HasAttachedPaymentMethod",
                table: "Organization");

            migrationBuilder.DropIndex(
                name: "IX_Organization_PaymentMethodEventRaisedAt",
                table: "Organization");

            migrationBuilder.DropIndex(
                name: "IX_JoinInvitation_Role",
                table: "JoinInvitation");

            migrationBuilder.DropIndex(
                name: "IX_IndustrySubCategory_Name",
                table: "IndustrySubCategory");

            migrationBuilder.DropIndex(
                name: "IX_IndustryMainCategory_Name",
                table: "IndustryMainCategory");

            migrationBuilder.DropIndex(
                name: "IX_AzureTenant_InstalledByUserId",
                table: "AzureTenant");

            migrationBuilder.DropIndex(
                name: "IX_AzureTenant_MembersLastRefreshedAt",
                table: "AzureTenant");

            migrationBuilder.DropIndex(
                name: "IX_AzureTenant_Name",
                table: "AzureTenant");
        }
    }
}
