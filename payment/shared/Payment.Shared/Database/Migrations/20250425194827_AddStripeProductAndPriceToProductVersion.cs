using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeProductAndPriceToProductVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "StripeCustomer",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripePriceId",
                table: "ProductVersion",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeProductId",
                table: "ProductVersion",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StripePrice",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StripePriceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripePrice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StripeProduct",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StripeProductId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripeProduct", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StripeCustomer_DeletedAt",
                table: "StripeCustomer",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_StripePriceId",
                table: "ProductVersion",
                column: "StripePriceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_StripeProductId",
                table: "ProductVersion",
                column: "StripeProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripePrice_CreatedAt",
                table: "StripePrice",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripePrice_DeletedAt",
                table: "StripePrice",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripePrice_ModifiedAt",
                table: "StripePrice",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripePrice_StripePriceId",
                table: "StripePrice",
                column: "StripePriceId");

            migrationBuilder.CreateIndex(
                name: "IX_StripeProduct_CreatedAt",
                table: "StripeProduct",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeProduct_DeletedAt",
                table: "StripeProduct",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeProduct_ModifiedAt",
                table: "StripeProduct",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeProduct_StripeProductId",
                table: "StripeProduct",
                column: "StripeProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVersion_StripePrice_StripePriceId",
                table: "ProductVersion",
                column: "StripePriceId",
                principalTable: "StripePrice",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVersion_StripeProduct_StripeProductId",
                table: "ProductVersion",
                column: "StripeProductId",
                principalTable: "StripeProduct",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVersion_StripePrice_StripePriceId",
                table: "ProductVersion");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVersion_StripeProduct_StripeProductId",
                table: "ProductVersion");

            migrationBuilder.DropTable(
                name: "StripePrice");

            migrationBuilder.DropTable(
                name: "StripeProduct");

            migrationBuilder.DropIndex(
                name: "IX_StripeCustomer_DeletedAt",
                table: "StripeCustomer");

            migrationBuilder.DropIndex(
                name: "IX_ProductVersion_StripePriceId",
                table: "ProductVersion");

            migrationBuilder.DropIndex(
                name: "IX_ProductVersion_StripeProductId",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "StripeCustomer");

            migrationBuilder.DropColumn(
                name: "StripePriceId",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "StripeProductId",
                table: "ProductVersion");
        }
    }
}
