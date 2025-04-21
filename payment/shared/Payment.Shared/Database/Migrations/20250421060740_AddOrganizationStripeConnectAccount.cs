using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
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
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ChargesEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    PayoutsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_ChargesEnabled",
                table: "OrganizationStripeConnectAccount",
                column: "ChargesEnabled");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_CreatedAt",
                table: "OrganizationStripeConnectAccount",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_DeletedAt",
                table: "OrganizationStripeConnectAccount",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_ModifiedAt",
                table: "OrganizationStripeConnectAccount",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_OrganizationId",
                table: "OrganizationStripeConnectAccount",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_PayoutsEnabled",
                table: "OrganizationStripeConnectAccount",
                column: "PayoutsEnabled");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_Type",
                table: "OrganizationStripeConnectAccount",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationStripeConnectAccount");
        }
    }
}
