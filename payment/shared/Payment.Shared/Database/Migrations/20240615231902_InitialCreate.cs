using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StripeCustomerId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Outbox",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Topic = table.Column<string>(type: "character varying(249)", maxLength: 249, nullable: false),
                    Headers = table.Column<Dictionary<string, string>>(type: "hstore", nullable: false),
                    Key = table.Column<byte[]>(type: "bytea", nullable: false),
                    Payload = table.Column<byte[]>(type: "bytea", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastRetry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    ProcessingErrors = table.Column<string>(type: "character varying(102400)", maxLength: 102400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CustomerId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Identity_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationMember",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MembershipType = table.Column<int>(type: "integer", nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    CustomerId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationMember_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationMember_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationOffering",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<int>(type: "integer", nullable: false),
                    Start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    End = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationOffering", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationOffering_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationStripePaymentMethod",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SetupIntentId = table.Column<string>(type: "text", nullable: true),
                    ClientSecret = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethodId = table.Column<string>(type: "text", nullable: true),
                    CardBrand = table.Column<string>(type: "text", nullable: true),
                    CardCountry = table.Column<string>(type: "text", nullable: true),
                    CardDescription = table.Column<string>(type: "text", nullable: true),
                    CardExpiryMonth = table.Column<byte>(type: "smallint", nullable: true),
                    CardExpiryYear = table.Column<short>(type: "smallint", nullable: true),
                    CardFingerprint = table.Column<string>(type: "text", nullable: true),
                    CardFunding = table.Column<string>(type: "text", nullable: true),
                    CardIssuer = table.Column<string>(type: "text", nullable: true),
                    CardLastFourDigit = table.Column<string>(type: "text", nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationStripePaymentMethod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationStripePaymentMethod_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationOfferingStripePaymentIntent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OrganizationStripePaymentMethodId = table.Column<string>(type: "character varying(100)", nullable: false),
                    OrganizationOfferingId = table.Column<string>(type: "character varying(100)", nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationOfferingStripePaymentIntent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationOfferingStripePaymentIntent_OrganizationOfferin~",
                        column: x => x.OrganizationOfferingId,
                        principalTable: "OrganizationOffering",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationOfferingStripePaymentIntent_OrganizationStripeP~",
                        column: x => x.OrganizationStripePaymentMethodId,
                        principalTable: "OrganizationStripePaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationOfferingStripePaymentIntent_Organization_Organi~",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customer_DeletedAt",
                table: "Customer",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_CustomerId",
                table: "Identity",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_DeletedAt",
                table: "Organization",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_Name",
                table: "Organization",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_CustomerId",
                table: "OrganizationMember",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_DeletedAt",
                table: "OrganizationMember",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_MembershipType",
                table: "OrganizationMember",
                column: "MembershipType");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_OrganizationId",
                table: "OrganizationMember",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_DeletedAt",
                table: "OrganizationOffering",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOffering_OrganizationId",
                table: "OrganizationOffering",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_DeletedAt",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_OrganizationId",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_OrganizationOfferin~",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "OrganizationOfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationOfferingStripePaymentIntent_OrganizationStripeP~",
                table: "OrganizationOfferingStripePaymentIntent",
                column: "OrganizationStripePaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentMethod_CardExpiryMonth_CardExpiryY~",
                table: "OrganizationStripePaymentMethod",
                columns: new[] { "CardExpiryMonth", "CardExpiryYear" });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentMethod_ClientSecret",
                table: "OrganizationStripePaymentMethod",
                column: "ClientSecret");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentMethod_DeletedAt",
                table: "OrganizationStripePaymentMethod",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentMethod_OrganizationId",
                table: "OrganizationStripePaymentMethod",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentMethod_PaymentMethodId",
                table: "OrganizationStripePaymentMethod",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentMethod_SetupIntentId",
                table: "OrganizationStripePaymentMethod",
                column: "SetupIntentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripePaymentMethod_Status",
                table: "OrganizationStripePaymentMethod",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Outbox_LastRetry",
                table: "Outbox",
                column: "LastRetry");

            migrationBuilder.CreateIndex(
                name: "IX_Outbox_RetryCount",
                table: "Outbox",
                column: "RetryCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Identity");

            migrationBuilder.DropTable(
                name: "OrganizationMember");

            migrationBuilder.DropTable(
                name: "OrganizationOfferingStripePaymentIntent");

            migrationBuilder.DropTable(
                name: "Outbox");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "OrganizationOffering");

            migrationBuilder.DropTable(
                name: "OrganizationStripePaymentMethod");

            migrationBuilder.DropTable(
                name: "Organization");
        }
    }
}
