using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountingInvoiceAndPaymentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountingContactLink",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Provider = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    LocalEntityType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    LocalEntityId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ExternalContactId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ExternalContactName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LastSyncedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastError = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingContactLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingContactLink_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountingInvoiceLink",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Provider = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    LocalEntityType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    LocalEntityId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ExternalInvoiceId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ExternalInvoiceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ExternalInvoiceUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ExternalStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
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
                    table.PrimaryKey("PK_AccountingInvoiceLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingInvoiceLink_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountingPaymentEvent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Provider = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ExternalInvoiceId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ExternalPaymentId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ExternalStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PayloadJson = table.Column<string>(type: "character varying(100000)", maxLength: 100000, nullable: true),
                    ProcessedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingPaymentEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingPaymentEvent_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingContactLink_CreatedAt",
                table: "AccountingContactLink",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingContactLink_ModifiedAt",
                table: "AccountingContactLink",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingContactLink_OrganizationId",
                table: "AccountingContactLink",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingContactLink_OrganizationId_Provider_ExternalConta~",
                table: "AccountingContactLink",
                columns: new[] { "OrganizationId", "Provider", "ExternalContactId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingContactLink_OrganizationId_Provider_LocalEntityTy~",
                table: "AccountingContactLink",
                columns: new[] { "OrganizationId", "Provider", "LocalEntityType", "LocalEntityId" },
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_AccountingPaymentEvent_CreatedAt",
                table: "AccountingPaymentEvent",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingPaymentEvent_ExternalInvoiceId",
                table: "AccountingPaymentEvent",
                column: "ExternalInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingPaymentEvent_ModifiedAt",
                table: "AccountingPaymentEvent",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingPaymentEvent_OrganizationId",
                table: "AccountingPaymentEvent",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingPaymentEvent_OrganizationId_Provider_ExternalPaym~",
                table: "AccountingPaymentEvent",
                columns: new[] { "OrganizationId", "Provider", "ExternalPaymentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingPaymentEvent_ProcessedAt",
                table: "AccountingPaymentEvent",
                column: "ProcessedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountingContactLink");

            migrationBuilder.DropTable(
                name: "AccountingInvoiceLink");

            migrationBuilder.DropTable(
                name: "AccountingPaymentEvent");
        }
    }
}
