using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Carrefour.API.BusinessIntelligence.Migrations
{
    /// <inheritdoc />
    public partial class CreateDailyConsolidatedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "daily_consolidated",
                columns: table => new
                {
                    id_daily_consolidated = table.Column<long>(type: "bigserial", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    value_debit = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    value_credit = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    debit_quantity = table.Column<int>(type: "integer", nullable: false),
                    credit_quantity = table.Column<int>(type: "integer", nullable: false),
                    consolidated_date = table.Column<DateOnly>(type: "date", nullable: false),
                    id_last_ledger_activity = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_consolidated", x => x.id_daily_consolidated);
                });

            migrationBuilder.CreateIndex(
                name: "IX_daily_consolidated_consolidated_date",
                table: "daily_consolidated",
                column: "consolidated_date",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "daily_consolidated");
        }
    }
}