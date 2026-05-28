using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProductToStripeCOnnectRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_StripeConnectAccount_OrganizationStripeConnectAccou~",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVersion_StripeConnectAccount_OrganizationStripeConne~",
                table: "ProductVersion");

            migrationBuilder.DropTable(
                name: "StripeConnectAccount");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "StripeConnectAccount",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_OrganizationStripeConnectAccountId",
                table: "ProductVersion",
                column: "OrganizationStripeConnectAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_OrganizationStripeConnectAccountId",
                table: "Product",
                column: "OrganizationStripeConnectAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_CreatedAt",
                table: "StripeConnectAccount",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_DeletedAt",
                table: "StripeConnectAccount",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_ModifiedAt",
                table: "StripeConnectAccount",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_OrganizationId",
                table: "StripeConnectAccount",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_StripeConnectAccount_OrganizationStripeConnectAccou~",
                table: "Product",
                column: "OrganizationStripeConnectAccountId",
                principalTable: "StripeConnectAccount",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVersion_StripeConnectAccount_OrganizationStripeConne~",
                table: "ProductVersion",
                column: "OrganizationStripeConnectAccountId",
                principalTable: "StripeConnectAccount",
                principalColumn: "Id");
        }
    }
}
