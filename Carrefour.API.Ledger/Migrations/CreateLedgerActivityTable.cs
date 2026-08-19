using Microsoft.EntityFrameworkCore.Migrations;

namespace Carrefour.API.Ledger.Migrations
{
    /// <inheritdoc />
    public partial class CreateLedgerActivityTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ledger_activity",
                columns: table => new
                {
                    id_ledger_activity = table.Column<long>(type: "bigserial", nullable: false),
                    operation = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    datetime_inclusion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ledger_activity", x => x.id_ledger_activity);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ledger_activity");
        }
    }
}
