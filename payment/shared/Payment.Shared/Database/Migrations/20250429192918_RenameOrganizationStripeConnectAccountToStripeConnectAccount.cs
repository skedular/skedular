using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrganizationStripeConnectAccountToStripeConnectAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVersion_OrganizationStripeConnectAccount_Organizatio~",
                table: "ProductVersion");

            migrationBuilder.DropTable(
                name: "OrganizationStripeConnectAccountRefreshCode");

            migrationBuilder.DropTable(
                name: "OrganizationStripeConnectAccount");

            migrationBuilder.CreateTable(
                name: "StripeConnectAccount",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StripeAccountId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ChargesEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PayoutsEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DefaultCurrency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    BusinessType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    Phone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DetailsSubmitted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ApplicationAuthorized = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CapabilitiesCardPayments = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CapabilitiesTransfers = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OnboardingUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripeConnectAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StripeConnectAccount_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StripeConnectAccountRefreshCode",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RedirectUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    StripeConnectAccountId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripeConnectAccountRefreshCode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StripeConnectAccountRefreshCode_StripeConnectAccount_Stripe~",
                        column: x => x.StripeConnectAccountId,
                        principalTable: "StripeConnectAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_ApplicationAuthorized",
                table: "StripeConnectAccount",
                column: "ApplicationAuthorized");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_BusinessType",
                table: "StripeConnectAccount",
                column: "BusinessType");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_CapabilitiesCardPayments",
                table: "StripeConnectAccount",
                column: "CapabilitiesCardPayments");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_CapabilitiesTransfers",
                table: "StripeConnectAccount",
                column: "CapabilitiesTransfers");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_ChargesEnabled",
                table: "StripeConnectAccount",
                column: "ChargesEnabled");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_Country",
                table: "StripeConnectAccount",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_CreatedAt",
                table: "StripeConnectAccount",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_DefaultCurrency",
                table: "StripeConnectAccount",
                column: "DefaultCurrency");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_DeletedAt",
                table: "StripeConnectAccount",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_DetailsSubmitted",
                table: "StripeConnectAccount",
                column: "DetailsSubmitted");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_Email",
                table: "StripeConnectAccount",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_ModifiedAt",
                table: "StripeConnectAccount",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_Name",
                table: "StripeConnectAccount",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_OrganizationId",
                table: "StripeConnectAccount",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_PayoutsEnabled",
                table: "StripeConnectAccount",
                column: "PayoutsEnabled");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_Phone",
                table: "StripeConnectAccount",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_StripeAccountId",
                table: "StripeConnectAccount",
                column: "StripeAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_Type",
                table: "StripeConnectAccount",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccountRefreshCode_Code",
                table: "StripeConnectAccountRefreshCode",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccountRefreshCode_CreatedAt",
                table: "StripeConnectAccountRefreshCode",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccountRefreshCode_DeletedAt",
                table: "StripeConnectAccountRefreshCode",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccountRefreshCode_ModifiedAt",
                table: "StripeConnectAccountRefreshCode",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccountRefreshCode_StripeConnectAccountId",
                table: "StripeConnectAccountRefreshCode",
                column: "StripeConnectAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVersion_StripeConnectAccount_OrganizationStripeConne~",
                table: "ProductVersion",
                column: "OrganizationStripeConnectAccountId",
                principalTable: "StripeConnectAccount",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVersion_StripeConnectAccount_OrganizationStripeConne~",
                table: "ProductVersion");

            migrationBuilder.DropTable(
                name: "StripeConnectAccountRefreshCode");

            migrationBuilder.DropTable(
                name: "StripeConnectAccount");

            migrationBuilder.CreateTable(
                name: "OrganizationStripeConnectAccount",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ApplicationAuthorized = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    BusinessType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CapabilitiesCardPayments = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CapabilitiesTransfers = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ChargesEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DefaultCurrency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DetailsSubmitted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OnboardingUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    PayoutsEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Phone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    StripeAccountId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationStripeConnectAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationStripeConnectAccount_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationStripeConnectAccountRefreshCode",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationStripeConnectAccountId = table.Column<string>(type: "character varying(100)", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RedirectUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationStripeConnectAccountRefreshCode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationStripeConnectAccountRefreshCode_OrganizationStr~",
                        column: x => x.OrganizationStripeConnectAccountId,
                        principalTable: "OrganizationStripeConnectAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_ApplicationAuthorized",
                table: "OrganizationStripeConnectAccount",
                column: "ApplicationAuthorized");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_BusinessType",
                table: "OrganizationStripeConnectAccount",
                column: "BusinessType");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_CapabilitiesCardPayments",
                table: "OrganizationStripeConnectAccount",
                column: "CapabilitiesCardPayments");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_CapabilitiesTransfers",
                table: "OrganizationStripeConnectAccount",
                column: "CapabilitiesTransfers");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_ChargesEnabled",
                table: "OrganizationStripeConnectAccount",
                column: "ChargesEnabled");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_Country",
                table: "OrganizationStripeConnectAccount",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_CreatedAt",
                table: "OrganizationStripeConnectAccount",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_DefaultCurrency",
                table: "OrganizationStripeConnectAccount",
                column: "DefaultCurrency");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_DeletedAt",
                table: "OrganizationStripeConnectAccount",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_DetailsSubmitted",
                table: "OrganizationStripeConnectAccount",
                column: "DetailsSubmitted");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_Email",
                table: "OrganizationStripeConnectAccount",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_ModifiedAt",
                table: "OrganizationStripeConnectAccount",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_Name",
                table: "OrganizationStripeConnectAccount",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_OrganizationId",
                table: "OrganizationStripeConnectAccount",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_PayoutsEnabled",
                table: "OrganizationStripeConnectAccount",
                column: "PayoutsEnabled");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_Phone",
                table: "OrganizationStripeConnectAccount",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_StripeAccountId",
                table: "OrganizationStripeConnectAccount",
                column: "StripeAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_Type",
                table: "OrganizationStripeConnectAccount",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccountRefreshCode_Code",
                table: "OrganizationStripeConnectAccountRefreshCode",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccountRefreshCode_CreatedAt",
                table: "OrganizationStripeConnectAccountRefreshCode",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccountRefreshCode_DeletedAt",
                table: "OrganizationStripeConnectAccountRefreshCode",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccountRefreshCode_ModifiedAt",
                table: "OrganizationStripeConnectAccountRefreshCode",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccountRefreshCode_OrganizationStr~",
                table: "OrganizationStripeConnectAccountRefreshCode",
                column: "OrganizationStripeConnectAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVersion_OrganizationStripeConnectAccount_Organizatio~",
                table: "ProductVersion",
                column: "OrganizationStripeConnectAccountId",
                principalTable: "OrganizationStripeConnectAccount",
                principalColumn: "Id");
        }
    }
}
