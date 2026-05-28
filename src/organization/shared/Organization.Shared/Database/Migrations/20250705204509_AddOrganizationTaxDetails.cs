using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationTaxDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationTaxDetails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    GstNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    GstPercentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTaxDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationTaxDetails_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTaxDetails_CreatedAt",
                table: "OrganizationTaxDetails",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTaxDetails_ModifiedAt",
                table: "OrganizationTaxDetails",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTaxDetails_OrganizationId",
                table: "OrganizationTaxDetails",
                column: "OrganizationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationTaxDetails");
        }
    }
}
