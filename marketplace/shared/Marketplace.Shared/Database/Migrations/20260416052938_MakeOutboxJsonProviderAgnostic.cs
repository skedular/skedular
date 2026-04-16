using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Temporalio.Client;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MakeOutboxJsonProviderAgnostic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:hstore", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AlterColumn<string>(
                name: "WorkflowSignalOptions",
                table: "TemporalSignalOutbox",
                type: "text",
                nullable: false,
                oldClrType: typeof(WorkflowSignalOptions),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "WorkflowOptions",
                table: "TemporalOutbox",
                type: "text",
                nullable: false,
                oldClrType: typeof(WorkflowOptions),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "Headers",
                table: "KafkaOutbox",
                type: "text",
                nullable: false,
                oldClrType: typeof(Dictionary<string, string>),
                oldType: "hstore");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AlterColumn<WorkflowSignalOptions>(
                name: "WorkflowSignalOptions",
                table: "TemporalSignalOutbox",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<WorkflowOptions>(
                name: "WorkflowOptions",
                table: "TemporalOutbox",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Dictionary<string, string>>(
                name: "Headers",
                table: "KafkaOutbox",
                type: "hstore",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
