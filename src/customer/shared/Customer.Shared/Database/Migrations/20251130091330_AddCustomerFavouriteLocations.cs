using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerFavouriteLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Location",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerLocation1",
                columns: table => new
                {
                    FavouredByCustomersId = table.Column<string>(type: "character varying(100)", nullable: false),
                    FavouriteLocationsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerLocation1", x => new { x.FavouredByCustomersId, x.FavouriteLocationsId });
                    table.ForeignKey(
                        name: "FK_CustomerLocation1_Customer_FavouredByCustomersId",
                        column: x => x.FavouredByCustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerLocation1_Location_FavouriteLocationsId",
                        column: x => x.FavouriteLocationsId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Location_Type",
                table: "Location",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLocation1_FavouriteLocationsId",
                table: "CustomerLocation1",
                column: "FavouriteLocationsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerLocation1");

            migrationBuilder.DropIndex(
                name: "IX_Location_Type",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Location");
        }
    }
}
