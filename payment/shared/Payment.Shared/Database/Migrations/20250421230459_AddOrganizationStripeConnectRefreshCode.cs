using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationStripeConnectRefreshCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationStripeConnectAccountRefreshCode",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
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
                name: "OrganizationStripeConnectAccountRefreshCode");
        }
    }
}
