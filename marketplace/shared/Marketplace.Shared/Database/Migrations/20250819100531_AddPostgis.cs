using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPostgis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:hstore", ",,");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:hstore", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");
        }
    }
}
