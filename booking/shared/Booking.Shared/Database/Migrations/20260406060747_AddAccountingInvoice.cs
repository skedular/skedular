using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountingInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountingInvoiceLink");

            migrationBuilder.CreateTable(
                name: "AccountingInvoiceExportLink",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Provider = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    LocalEntityType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    LocalEntityId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ExternalInvoiceId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ExternalInvoiceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ExternalInvoiceUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ExternalInvoiceMode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ExternalStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ExportConfigurationState = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ExportConfigurationMessage = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    RepeatingScheduleSource = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    RepeatingScheduleUnit = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    RepeatingSchedulePeriod = table.Column<int>(type: "integer", nullable: true),
                    SentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PaidAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastSyncedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastError = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingInvoiceExportLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingInvoiceExportLink_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountingInvoiceInstance",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Provider = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ExternalInvoiceId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ExternalInvoiceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ExternalInvoiceUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ExternalStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PaidAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastSyncedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastError = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    AccountingInvoiceExportLinkId = table.Column<string>(type: "character varying(100)", nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingInvoiceInstance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingInvoiceInstance_AccountingInvoiceExportLink_Accou~",
                        column: x => x.AccountingInvoiceExportLinkId,
                        principalTable: "AccountingInvoiceExportLink",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountingInvoiceInstance_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingInvoiceExportLink_CreatedAt",
                table: "AccountingInvoiceExportLink",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingInvoiceExportLink_ExternalStatus",
                table: "AccountingInvoiceExportLink",
                column: "ExternalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingInvoiceExportLink_ModifiedAt",
                table: "AccountingInvoiceExportLink",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingInvoiceExportLink_OrganizationId",
                table: "AccountingInvoiceExportLink",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingInvoiceExportLink_Provider_ExternalInvoiceId",
                table: "AccountingInvoiceExportLink",
                columns: new[] { "Provider", "ExternalInvoiceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingInvoiceExportLink_Provider_LocalEntityType_LocalE~",
                table: "AccountingInvoiceExportLink",
                columns: new[] { "Provider", "LocalEntityType", "LocalEntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingInvoiceInstance_AccountingInvoiceExportLinkId",
                table: "AccountingInvoiceInstance",
                column: "AccountingInvoiceExportLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingInvoiceInstance_CreatedAt",
                table: "AccountingInvoiceInstance",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingInvoiceInstance_ExternalStatus",
                table: "AccountingInvoiceInstance",
                column: "ExternalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingInvoiceInstance_ModifiedAt",
                table: "AccountingInvoiceInstance",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingInvoiceInstance_OrganizationId",
                table: "AccountingInvoiceInstance",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingInvoiceInstance_Provider_ExternalInvoiceId",
                table: "AccountingInvoiceInstance",
                columns: new[] { "Provider", "ExternalInvoiceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountingInvoiceInstance");

            migrationBuilder.DropTable(
                name: "AccountingInvoiceExportLink");

            migrationBuilder.CreateTable(
                name: "AccountingInvoiceLink",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    ExportConfigurationMessage = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    ExportConfigurationState = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ExternalInvoiceId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ExternalInvoiceMode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ExternalInvoiceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ExternalInvoiceUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ExternalStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    LastError = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    LastSyncedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LocalEntityId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LocalEntityType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PaidAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Provider = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    RepeatingSchedulePeriod = table.Column<int>(type: "integer", nullable: true),
                    RepeatingScheduleSource = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    RepeatingScheduleUnit = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    SentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingInvoiceLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingInvoiceLink_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingInvoiceLink_CreatedAt",
                table: "AccountingInvoiceLink",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingInvoiceLink_ExternalStatus",
                table: "AccountingInvoiceLink",
                column: "ExternalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingInvoiceLink_ModifiedAt",
                table: "AccountingInvoiceLink",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingInvoiceLink_OrganizationId",
                table: "AccountingInvoiceLink",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingInvoiceLink_Provider_ExternalInvoiceId",
                table: "AccountingInvoiceLink",
                columns: new[] { "Provider", "ExternalInvoiceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingInvoiceLink_Provider_LocalEntityType_LocalEntityId",
                table: "AccountingInvoiceLink",
                columns: new[] { "Provider", "LocalEntityType", "LocalEntityId" },
                unique: true);
        }
    }
}
