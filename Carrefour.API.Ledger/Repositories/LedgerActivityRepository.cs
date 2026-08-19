using Carrefour.API.Ledger.DTOs;
using Carrefour.API.Ledger.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Carrefour.API.Ledger.Repositories
{
    public class LedgerActivityRepository : ILedgerActivityRepository
    {
        private readonly Context _dbContext;
        public LedgerActivityRepository(Context dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<LedgerActivity> CreateAsync(LedgerActivity ledgerActivity, CancellationToken ct = default)
        {
            await _dbContext.ledgerActivity.AddAsync(ledgerActivity, ct);
            await _dbContext.SaveChangesAsync(ct);
            return ledgerActivity;
        }

        public async Task<IEnumerable<LedgerActivity>> ReadAllAsync(CancellationToken ct = default)
        {
            return await _dbContext.ledgerActivity.AsNoTracking().ToListAsync(ct);
        }
    }
}