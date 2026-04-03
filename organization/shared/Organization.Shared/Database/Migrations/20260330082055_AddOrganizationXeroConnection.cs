using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationXeroConnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationXeroConnection",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TenantName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BillingMode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Disabled"),
                    Scopes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    SendInvoicesViaXero = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    AutoReconcilePayments = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    DefaultSalesAccountCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DefaultReceivablesAccountCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DefaultTrackingCategory1 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DefaultTrackingCategory2 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DefaultBrandingThemeId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DefaultReferencePrefix = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AccessTokenEncrypted = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    RefreshTokenEncrypted = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    AccessTokenExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RefreshTokenExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastSuccessfulSyncAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastError = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationXeroConnection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationXeroConnection_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationXeroConnection_BillingMode",
                table: "OrganizationXeroConnection",
                column: "BillingMode");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationXeroConnection_CreatedAt",
                table: "OrganizationXeroConnection",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationXeroConnection_IsActive",
                table: "OrganizationXeroConnection",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationXeroConnection_ModifiedAt",
                table: "OrganizationXeroConnection",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationXeroConnection_OrganizationId",
                table: "OrganizationXeroConnection",
                column: "OrganizationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationXeroConnection_TenantId",
                table: "OrganizationXeroConnection",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationXeroConnection");
        }
    }
}
