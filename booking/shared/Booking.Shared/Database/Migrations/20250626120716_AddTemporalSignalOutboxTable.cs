using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Temporalio.Client;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTemporalSignalOutboxTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TemporalSignalOutbox",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    WorkflowId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SignalType = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    ExecutionArgs = table.Column<string>(type: "character varying(10240)", maxLength: 10240, nullable: true),
                    WorkflowSignalOptions = table.Column<WorkflowSignalOptions>(type: "jsonb", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastRetry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    ProcessingErrors = table.Column<string>(type: "character varying(102400)", maxLength: 102400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemporalSignalOutbox", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemporalSignalOutbox_LastRetry",
                table: "TemporalSignalOutbox",
                column: "LastRetry");

            migrationBuilder.CreateIndex(
                name: "IX_TemporalSignalOutbox_RetryCount",
                table: "TemporalSignalOutbox",
                column: "RetryCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemporalSignalOutbox");
        }
    }
}
