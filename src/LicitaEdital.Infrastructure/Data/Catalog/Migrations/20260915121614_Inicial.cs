using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitaEdital.Infrastructure.Data.Catalog.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "catalog");

            migrationBuilder.CreateTable(
                name: "opportunities",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    source = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    external_reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    title = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    @object = table.Column<string>(name: "object", type: "character varying(2000)", maxLength: 2000, nullable: false),
                    buyer_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    contract_number = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    state = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    city = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    city_ibge_code = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    modality_code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    modality_label = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    estimated_value_cents = table.Column<long>(type: "bigint", nullable: true),
                    published_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    proposal_deadline = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    official_url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    collected_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("pk_opportunities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "opportunity_compatibilities",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    opportunity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    offering_id = table.Column<Guid>(type: "uuid", nullable: true),
                    offering_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    score = table.Column<int>(type: "integer", nullable: true),
                    engine_version = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    calculated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    attention_points = table.Column<List<string>>(type: "text[]", nullable: false),
                    matched_terms = table.Column<List<string>>(type: "text[]", nullable: false),
                    positive_reasons = table.Column<List<string>>(type: "text[]", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_opportunity_compatibilities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "opportunity_documents",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    label = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    published_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    opportunity_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_opportunity_documents", x => x.id);
                    table.ForeignKey(
                        name: "fk_opportunity_documents_opportunities_opportunity_id",
                        column: x => x.opportunity_id,
                        principalSchema: "catalog",
                        principalTable: "opportunities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "opportunity_line_items",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    unit = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    unit_value_cents = table.Column<long>(type: "bigint", nullable: true),
                    total_value_cents = table.Column<long>(type: "bigint", nullable: true),
                    catalog_code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    opportunity_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_opportunity_line_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_opportunity_line_items_opportunities_opportunity_id",
                        column: x => x.opportunity_id,
                        principalSchema: "catalog",
                        principalTable: "opportunities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_opportunities_published_at",
                schema: "catalog",
                table: "opportunities",
                column: "published_at");

            migrationBuilder.CreateIndex(
                name: "ix_opportunities_state_status_deadline",
                schema: "catalog",
                table: "opportunities",
                columns: new[] { "state", "status", "proposal_deadline" });

            migrationBuilder.CreateIndex(
                name: "ux_opportunities_source_external_reference",
                schema: "catalog",
                table: "opportunities",
                columns: new[] { "source", "external_reference" },
                unique: true,
                filter: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_opportunity_compatibilities_engine_version",
                schema: "catalog",
                table: "opportunity_compatibilities",
                column: "engine_version");

            migrationBuilder.CreateIndex(
                name: "ix_opportunity_compatibilities_organization_score",
                schema: "catalog",
                table: "opportunity_compatibilities",
                columns: new[] { "organization_id", "score" });

            migrationBuilder.CreateIndex(
                name: "ux_opportunity_compatibilities_organization_opportunity",
                schema: "catalog",
                table: "opportunity_compatibilities",
                columns: new[] { "organization_id", "opportunity_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_opportunity_documents_opportunity",
                schema: "catalog",
                table: "opportunity_documents",
                column: "opportunity_id");

            migrationBuilder.CreateIndex(
                name: "ux_opportunity_line_items_opportunity_number",
                schema: "catalog",
                table: "opportunity_line_items",
                columns: new[] { "opportunity_id", "number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "opportunity_compatibilities",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "opportunity_documents",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "opportunity_line_items",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "opportunities",
                schema: "catalog");
        }
    }
}
