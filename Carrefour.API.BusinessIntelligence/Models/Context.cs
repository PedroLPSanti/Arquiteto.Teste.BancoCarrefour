using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace Carrefour.API.BusinessIntelligence.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options) { }

        public DbSet<DailyConsolidated> DailyConsolidated { get; set; }
    }
}
