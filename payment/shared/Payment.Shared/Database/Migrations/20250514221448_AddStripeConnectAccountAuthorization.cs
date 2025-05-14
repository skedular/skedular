using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeConnectAccountAuthorization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StripeConnectAccount_ApplicationAuthorized",
                table: "StripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "ApplicationAuthorized",
                table: "StripeConnectAccount");

            migrationBuilder.CreateTable(
                name: "StripeConnectAccountAuthorization",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsAuthorized = table.Column<bool>(type: "boolean", nullable: false),
                    StripeConnectAccountId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripeConnectAccountAuthorization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StripeConnectAccountAuthorization_StripeConnectAccount_Stri~",
                        column: x => x.StripeConnectAccountId,
                        principalTable: "StripeConnectAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccountAuthorization_CreatedAt",
                table: "StripeConnectAccountAuthorization",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccountAuthorization_IsAuthorized",
                table: "StripeConnectAccountAuthorization",
                column: "IsAuthorized");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccountAuthorization_ModifiedAt",
                table: "StripeConnectAccountAuthorization",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccountAuthorization_StripeConnectAccountId",
                table: "StripeConnectAccountAuthorization",
                column: "StripeConnectAccountId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StripeConnectAccountAuthorization");

            migrationBuilder.AddColumn<bool>(
                name: "ApplicationAuthorized",
                table: "StripeConnectAccount",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_StripeConnectAccount_ApplicationAuthorized",
                table: "StripeConnectAccount",
                column: "ApplicationAuthorized");
        }
    }
}
