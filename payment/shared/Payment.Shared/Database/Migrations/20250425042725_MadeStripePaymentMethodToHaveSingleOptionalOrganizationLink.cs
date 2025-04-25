using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MadeStripePaymentMethodToHaveSingleOptionalOrganizationLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationStripePaymentMethod");

            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "StripePaymentMethod",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentMethod_OrganizationId",
                table: "StripePaymentMethod",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_StripePaymentMethod_Organization_OrganizationId",
                table: "StripePaymentMethod",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StripePaymentMethod_Organization_OrganizationId",
                table: "StripePaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_StripePaymentMethod_OrganizationId",
                table: "StripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "StripePaymentMethod");

            migrationBuilder.CreateTable(
                name: "OrganizationStripePaymentMethod",
                columns: table => new
                {
                    OrganizationsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    StripePaymentMethodsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationStripePaymentMethod", x => new { x.OrganizationsId, x.StripePaymentMethodsId });
                    table.ForeignKey(
                        name: "FK_OrganizationStripePaymentMethod_Organization_OrganizationsId",
                        column: x => x.OrganizationsId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationStripePaymentMethod_StripePaymentMethod_StripeP~",
                        column: x => x.StripePaymentMethodsId,
                        principalTable: "StripePaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentMethod_StripePaymentMethodsId",
                table: "OrganizationStripePaymentMethod",
                column: "StripePaymentMethodsId");
        }
    }
}
