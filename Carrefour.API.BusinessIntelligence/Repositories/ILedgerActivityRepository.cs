using Carrefour.API.BusinessIntelligence.Models;
using Microsoft.EntityFrameworkCore;

namespace Carrefour.API.BusinessIntelligence.Repositories
{
    public interface IDailyConsolidatedRepository
    {
        public Task<IEnumerable<DailyConsolidated>> ReadAllAsync(CancellationToken ct = default);
    }
}
