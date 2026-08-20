using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Carrefour.ETL.BusinessIntelligence.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<LedgerActivity> ledgerActivity { get; set; }
        public DbSet<DailyConsolidated> dailyConsolidated { get; set; }
    }
}