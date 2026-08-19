using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Carrefour.API.Ledger.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<LedgerActivity> ledgerActivity { get; set; }
    }
}