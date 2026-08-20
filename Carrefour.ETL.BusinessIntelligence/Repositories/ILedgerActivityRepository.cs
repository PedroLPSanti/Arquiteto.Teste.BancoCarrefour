using Carrefour.ETL.BusinessIntelligence.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Carrefour.ETL.BusinessIntelligence.Repositories
{
    public interface ILedgerActivityRepository
    {
        Task<List<DailyConsolidated>> ExtractAndTransformIncrementalAsync(long lastProcessedLedgerId, CancellationToken ct = default);
    }
}
