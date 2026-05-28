using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationStripeConnectAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizationStripeConnectAccountId",
                table: "ProductVersion",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationStripeConnectAccountId",
                table: "Product",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrganizationStripeConnectAccount",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
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
                name: "IX_ProductVersion_OrganizationStripeConnectAccountId",
                table: "ProductVersion",
                column: "OrganizationStripeConnectAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_OrganizationStripeConnectAccountId",
                table: "Product",
                column: "OrganizationStripeConnectAccountId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Product_OrganizationStripeConnectAccount_OrganizationStripe~",
                table: "Product",
                column: "OrganizationStripeConnectAccountId",
                principalTable: "OrganizationStripeConnectAccount",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVersion_OrganizationStripeConnectAccount_Organizatio~",
                table: "ProductVersion",
                column: "OrganizationStripeConnectAccountId",
                principalTable: "OrganizationStripeConnectAccount",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_OrganizationStripeConnectAccount_OrganizationStripe~",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVersion_OrganizationStripeConnectAccount_Organizatio~",
                table: "ProductVersion");

            migrationBuilder.DropTable(
                name: "OrganizationStripeConnectAccount");

            migrationBuilder.DropIndex(
                name: "IX_ProductVersion_OrganizationStripeConnectAccountId",
                table: "ProductVersion");

            migrationBuilder.DropIndex(
                name: "IX_Product_OrganizationStripeConnectAccountId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "OrganizationStripeConnectAccountId",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "OrganizationStripeConnectAccountId",
                table: "Product");
        }
    }
}
