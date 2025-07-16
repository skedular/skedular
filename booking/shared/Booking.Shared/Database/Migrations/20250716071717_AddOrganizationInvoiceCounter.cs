using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationInvoiceCounter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationInvoiceCounter",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    InvoiceNumber = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationInvoiceCounter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationInvoiceCounter_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationInvoiceCounter_CreatedAt",
                table: "OrganizationInvoiceCounter",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationInvoiceCounter_ModifiedAt",
                table: "OrganizationInvoiceCounter",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationInvoiceCounter_OrganizationId",
                table: "OrganizationInvoiceCounter",
                column: "OrganizationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationInvoiceCounter");
        }
    }
}
