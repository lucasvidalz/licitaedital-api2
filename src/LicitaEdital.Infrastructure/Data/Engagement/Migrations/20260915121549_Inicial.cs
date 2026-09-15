using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitaEdital.Infrastructure.Data.Engagement.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "engagement");

            migrationBuilder.CreateTable(
                name: "alert_preferences",
                schema: "engagement",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    alert_new_compatible_opportunity = table.Column<bool>(type: "boolean", nullable: false),
                    alert_approaching_deadline = table.Column<bool>(type: "boolean", nullable: false),
                    alert_daily_summary = table.Column<bool>(type: "boolean", nullable: false),
                    frequency = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    filter_value_range = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    filter_modalities = table.Column<List<string>>(type: "text[]", nullable: false),
                    filter_states = table.Column<string[]>(type: "text[]", nullable: false),
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
                    table.PrimaryKey("pk_alert_preferences", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "saved_opportunities",
                schema: "engagement",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    opportunity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    saved_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_saved_opportunities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                schema: "engagement",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("pk_subscriptions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ux_alert_preferences_organization",
                schema: "engagement",
                table: "alert_preferences",
                column: "organization_id",
                unique: true,
                filter: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_saved_opportunities_organization_created_at",
                schema: "engagement",
                table: "saved_opportunities",
                columns: new[] { "organization_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ux_saved_opportunities_organization_opportunity",
                schema: "engagement",
                table: "saved_opportunities",
                columns: new[] { "organization_id", "opportunity_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_subscriptions_organization",
                schema: "engagement",
                table: "subscriptions",
                column: "organization_id",
                unique: true,
                filter: "is_active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alert_preferences",
                schema: "engagement");

            migrationBuilder.DropTable(
                name: "saved_opportunities",
                schema: "engagement");

            migrationBuilder.DropTable(
                name: "subscriptions",
                schema: "engagement");
        }
    }
}
