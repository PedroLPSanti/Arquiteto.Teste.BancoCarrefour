using Carrefour.ETL.BusinessIntelligence.Enums;
using Carrefour.ETL.BusinessIntelligence.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Carrefour.ETL.BusinessIntelligence.Repositories
{
    public class LedgerActivityRepository : ILedgerActivityRepository
    {
        private readonly Context _dbContext;
        public LedgerActivityRepository(Context dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DailyConsolidated>> ExtractAndTransformIncrementalAsync(long lastProcessedLedgerId, CancellationToken ct = default)
        {
            return await _dbContext.Set<LedgerActivity>()
                .AsNoTracking()
                .Where(x => x.idLedgerActivity > lastProcessedLedgerId)
                .GroupBy(x => DateOnly.FromDateTime(x.dateTimeInclusion))
                .Select(g => new DailyConsolidated
                {
                    consolidatedDate = g.Key,
                    valueDebit = g.Where(x => x.operation == OperationEnum.DEBIT).Sum(x => x.value),
                    valueCredit = g.Where(x => x.operation == OperationEnum.CREDIT).Sum(x => x.value),
                    debitQuantity = g.Count(x => x.operation == OperationEnum.DEBIT),
                    creditQuantity = g.Count(x => x.operation == OperationEnum.CREDIT),
                    idLastLedgerActivity = g.Max(x => x.idLedgerActivity)
                })
                .OrderBy(x => x.consolidatedDate)
                .ToListAsync(ct);
        }
    }
}