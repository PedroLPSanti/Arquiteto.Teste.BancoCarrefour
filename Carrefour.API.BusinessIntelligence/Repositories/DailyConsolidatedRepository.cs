using System.Linq;
using Carrefour.API.BusinessIntelligence.DTOs;
using Carrefour.API.BusinessIntelligence.Models;
using Microsoft.EntityFrameworkCore;

namespace Carrefour.API.BusinessIntelligence.Repositories
{
    public class DailyConsolidatedRepository : IDailyConsolidatedRepository
    {
        private readonly Context _dbContext;

        public DailyConsolidatedRepository(Context dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<DailyConsolidated>> ReadAllAsync(
            CancellationToken ct = default
        )
        {
            return await _dbContext.DailyConsolidated.AsNoTracking().ToListAsync(ct);
        }
    }
}
