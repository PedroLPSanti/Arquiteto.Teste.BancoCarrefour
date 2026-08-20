using Carrefour.ETL.BusinessIntelligence.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Carrefour.ETL.BusinessIntelligence.Repositories
{
    public interface IDailyConsolidatedRepository
    {
        public Task<long> GetLastProcessedLedgerIdAsync(CancellationToken ct = default);

        public Task<Dictionary<DateOnly, DailyConsolidated>> GetExistingRecordsByDatesAsync(
        IEnumerable<DateOnly> dates,
        CancellationToken ct = default);

        public Task SaveChangesAsync(CancellationToken ct = default);

        public Task CreateAsync(DailyConsolidated dailyConsolidated, CancellationToken ct = default);
    }
}
