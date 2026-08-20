using Carrefour.ETL.BusinessIntelligence.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Carrefour.ETL.BusinessIntelligence.Repositories
{
    public class DailyConsolidatedRepository : IDailyConsolidatedRepository
    {
        private readonly Context _dbContext;

        public DailyConsolidatedRepository(Context dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<long> GetLastProcessedLedgerIdAsync(CancellationToken ct = default)
        {
            return await _dbContext.Set<DailyConsolidated>()
                .AsNoTracking()
                .MaxAsync(x => (long?)x.idLastLedgerActivity, ct) ?? 0;
        }

        public async Task<Dictionary<DateOnly, DailyConsolidated>> GetExistingRecordsByDatesAsync(
        IEnumerable<DateOnly> dates,
        CancellationToken ct = default)
        {
            return await _dbContext.Set<DailyConsolidated>()
                .Where(x => dates.Contains(x.consolidatedDate))
                .ToDictionaryAsync(x => x.consolidatedDate, ct);
        }


        public async Task CreateAsync(DailyConsolidated dailyConsolidated, CancellationToken ct = default)
        {
            await _dbContext.dailyConsolidated.AddAsync(dailyConsolidated, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
