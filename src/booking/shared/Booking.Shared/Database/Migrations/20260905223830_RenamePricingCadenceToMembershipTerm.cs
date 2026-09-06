using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenamePricingCadenceToMembershipTerm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PricingCadence",
                table: "StripeProduct",
                newName: "MembershipTerm");

            migrationBuilder.RenameIndex(
                name: "IX_StripeProduct_PricingCadence",
                table: "StripeProduct",
                newName: "IX_StripeProduct_MembershipTerm");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MembershipTerm",
                table: "StripeProduct",
                newName: "PricingCadence");

            migrationBuilder.RenameIndex(
                name: "IX_StripeProduct_MembershipTerm",
                table: "StripeProduct",
                newName: "IX_StripeProduct_PricingCadence");
        }
    }
}
