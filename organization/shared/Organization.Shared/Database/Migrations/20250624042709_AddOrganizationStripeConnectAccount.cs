using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationStripeConnectAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationStripeConnectAccount",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    StripeAccountId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ChargesEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PayoutsEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DefaultCurrency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    BusinessType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SupportUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ContactEmail = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    ContactPhone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    DetailsSubmitted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CapabilitiesCardPayments = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CapabilitiesTransfers = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OnboardingUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
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
                name: "OrganizationStripeConnectAccountAuthorization",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsAuthorized = table.Column<bool>(type: "boolean", nullable: false),
                    OrganizationStripeConnectAccountId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationStripeConnectAccountAuthorization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationStripeConnectAccountAuthorization_OrganizationS~",
                        column: x => x.OrganizationStripeConnectAccountId,
                        principalTable: "OrganizationStripeConnectAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationStripeConnectAccountRefreshCode",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RedirectUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    OrganizationStripeConnectAccountId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
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
                name: "IX_OrganizationStripeConnectAccount_IsDefault",
                table: "OrganizationStripeConnectAccount",
                column: "IsDefault");

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
                name: "IX_OrganizationStripeConnectAccount_StripeAccountId",
                table: "OrganizationStripeConnectAccount",
                column: "StripeAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_Type",
                table: "OrganizationStripeConnectAccount",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccountAuthorization_CreatedAt",
                table: "OrganizationStripeConnectAccountAuthorization",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccountAuthorization_IsAuthorized",
                table: "OrganizationStripeConnectAccountAuthorization",
                column: "IsAuthorized");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccountAuthorization_ModifiedAt",
                table: "OrganizationStripeConnectAccountAuthorization",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccountAuthorization_OrganizationS~",
                table: "OrganizationStripeConnectAccountAuthorization",
                column: "OrganizationStripeConnectAccountId",
                unique: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationStripeConnectAccountAuthorization");

            migrationBuilder.DropTable(
                name: "OrganizationStripeConnectAccountRefreshCode");

            migrationBuilder.DropTable(
                name: "OrganizationStripeConnectAccount");
        }
    }
}
