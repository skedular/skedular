using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Temporalio.Client;

#nullable disable

namespace Billing.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTemporalOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TemporalOutbox",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    WorkflowType = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    ExecutionArgs = table.Column<string>(type: "character varying(10240)", maxLength: 10240, nullable: true),
                    WorkflowOptions = table.Column<WorkflowOptions>(type: "jsonb", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastRetry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    ProcessingErrors = table.Column<string>(type: "character varying(102400)", maxLength: 102400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemporalOutbox", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemporalOutbox_LastRetry",
                table: "TemporalOutbox",
                column: "LastRetry");

            migrationBuilder.CreateIndex(
                name: "IX_TemporalOutbox_RetryCount",
                table: "TemporalOutbox",
                column: "RetryCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemporalOutbox");
        }
    }
}
