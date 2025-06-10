using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RestructuredIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationOffering_OrganizationOfferingStripePaymentInten~",
                table: "OrganizationOffering");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationOfferingStripePaymentIntent_StripePaymentMethod~",
                table: "OrganizationOfferingStripePaymentIntent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationOfferingStripePaymentIntent",
                table: "OrganizationOfferingStripePaymentIntent");

            migrationBuilder.RenameTable(
                name: "OrganizationOfferingStripePaymentIntent",
                newName: "StripePaymentIntent");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_StripePaymentMethod~",
                table: "StripePaymentIntent",
                newName: "IX_StripePaymentIntent_StripePaymentMethodId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_ModifiedAt",
                table: "StripePaymentIntent",
                newName: "IX_StripePaymentIntent_ModifiedAt");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_DeletedAt",
                table: "StripePaymentIntent",
                newName: "IX_StripePaymentIntent_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_Currency",
                table: "StripePaymentIntent",
                newName: "IX_StripePaymentIntent_Currency");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_CreatedAt",
                table: "StripePaymentIntent",
                newName: "IX_StripePaymentIntent_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_Amount",
                table: "StripePaymentIntent",
                newName: "IX_StripePaymentIntent_Amount");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StripePaymentIntent",
                table: "StripePaymentIntent",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationOffering_StripePaymentIntent_StripePaymentInten~",
                table: "OrganizationOffering",
                column: "StripePaymentIntentId",
                principalTable: "StripePaymentIntent",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StripePaymentIntent_StripePaymentMethod_StripePaymentMethod~",
                table: "StripePaymentIntent",
                column: "StripePaymentMethodId",
                principalTable: "StripePaymentMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationOffering_StripePaymentIntent_StripePaymentInten~",
                table: "OrganizationOffering");

            migrationBuilder.DropForeignKey(
                name: "FK_StripePaymentIntent_StripePaymentMethod_StripePaymentMethod~",
                table: "StripePaymentIntent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StripePaymentIntent",
                table: "StripePaymentIntent");

            migrationBuilder.RenameTable(
                name: "StripePaymentIntent",
                newName: "OrganizationOfferingStripePaymentIntent");

            migrationBuilder.RenameIndex(
                name: "IX_StripePaymentIntent_StripePaymentMethodId",
                table: "OrganizationOfferingStripePaymentIntent",
                newName: "IX_OrganizationOfferingStripePaymentIntent_StripePaymentMethod~");

            migrationBuilder.RenameIndex(
                name: "IX_StripePaymentIntent_ModifiedAt",
                table: "OrganizationOfferingStripePaymentIntent",
                newName: "IX_OrganizationOfferingStripePaymentIntent_ModifiedAt");

            migrationBuilder.RenameIndex(
                name: "IX_StripePaymentIntent_DeletedAt",
                table: "OrganizationOfferingStripePaymentIntent",
                newName: "IX_OrganizationOfferingStripePaymentIntent_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_StripePaymentIntent_Currency",
                table: "OrganizationOfferingStripePaymentIntent",
                newName: "IX_OrganizationOfferingStripePaymentIntent_Currency");

            migrationBuilder.RenameIndex(
                name: "IX_StripePaymentIntent_CreatedAt",
                table: "OrganizationOfferingStripePaymentIntent",
                newName: "IX_OrganizationOfferingStripePaymentIntent_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_StripePaymentIntent_Amount",
                table: "OrganizationOfferingStripePaymentIntent",
                newName: "IX_OrganizationOfferingStripePaymentIntent_Amount");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationOfferingStripePaymentIntent",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationOffering_OrganizationOfferingStripePaymentInten~",
                table: "OrganizationOffering",
                column: "StripePaymentIntentId",
                principalTable: "OrganizationOfferingStripePaymentIntent",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationOfferingStripePaymentIntent_StripePaymentMethod~",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "StripePaymentMethodId",
                principalTable: "StripePaymentMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
