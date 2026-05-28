using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedPendingEntityChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "Coordinates",
                table: "LocationPhysicalAddress",
                type: "geometry (point, 4326)",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geometry(point, 4326)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "Coordinates",
                table: "LocationPhysicalAddress",
                type: "geometry(point, 4326)",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geometry (point, 4326)",
                oldNullable: true);
        }
    }
}
