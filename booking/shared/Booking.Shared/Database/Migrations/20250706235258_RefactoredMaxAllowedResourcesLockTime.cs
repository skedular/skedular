using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredMaxAllowedResourcesLockTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxAllowedResourcesLockTimePaidThroughBankAccount",
                table: "ProductVersion",
                newName: "MaxAllowedResourcesLockTimePaidViaBankTransfer");

            migrationBuilder.RenameColumn(
                name: "MaxAllowedResourcesLockTimePaidByCard",
                table: "ProductVersion",
                newName: "MaxAllowedResourcesLockTimePaidViaCard");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxAllowedResourcesLockTimePaidViaCard",
                table: "ProductVersion",
                newName: "MaxAllowedResourcesLockTimePaidByCard");

            migrationBuilder.RenameColumn(
                name: "MaxAllowedResourcesLockTimePaidViaBankTransfer",
                table: "ProductVersion",
                newName: "MaxAllowedResourcesLockTimePaidThroughBankAccount");
        }
    }
}
