using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitaEdital.Infrastructure.Data.Collections.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "collections");

            migrationBuilder.CreateTable(
                name: "collection_runs",
                schema: "collections",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    new_opportunities_count = table.Column<int>(type: "integer", nullable: false),
                    result = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_collection_runs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "coverage_settings",
                schema: "collections",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    value_range = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    attended_states = table.Column<string[]>(type: "text[]", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_coverage_settings", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_collection_runs_result_ended_at",
                schema: "collections",
                table: "collection_runs",
                columns: new[] { "result", "ended_at" });

            migrationBuilder.CreateIndex(
                name: "ix_collection_runs_started_at",
                schema: "collections",
                table: "collection_runs",
                column: "started_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "collection_runs",
                schema: "collections");

            migrationBuilder.DropTable(
                name: "coverage_settings",
                schema: "collections");
        }
    }
}
