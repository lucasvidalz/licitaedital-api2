using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitaEdital.Infrastructure.Data.Offerings.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "offerings");

            migrationBuilder.CreateTable(
                name: "offerings",
                schema: "offerings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    supply_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    min_value_cents = table.Column<long>(type: "bigint", nullable: true),
                    max_value_cents = table.Column<long>(type: "bigint", nullable: true),
                    catalog_codes = table.Column<List<string>>(type: "text[]", nullable: false),
                    negative_keywords = table.Column<List<string>>(type: "text[]", nullable: false),
                    positive_keywords = table.Column<List<string>>(type: "text[]", nullable: false),
                    served_regions = table.Column<string[]>(type: "text[]", nullable: false),
                    synonyms = table.Column<List<string>>(type: "text[]", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_offerings", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_offerings_organization",
                schema: "offerings",
                table: "offerings",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ux_offerings_organization_name",
                schema: "offerings",
                table: "offerings",
                columns: new[] { "organization_id", "name" },
                unique: true,
                filter: "is_active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "offerings",
                schema: "offerings");
        }
    }
}
