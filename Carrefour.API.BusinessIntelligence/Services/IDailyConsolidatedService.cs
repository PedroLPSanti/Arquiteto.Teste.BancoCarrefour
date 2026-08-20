using Carrefour.API.BusinessIntelligence.DTOs;
using Carrefour.API.BusinessIntelligence.Models;
using Microsoft.EntityFrameworkCore;

namespace Carrefour.API.BusinessIntelligence.Repositories
{
    public interface IDailyConsolidatedService
    {
        Task<IEnumerable<DailyConsolidatedDTO>> ReadAllAsync(CancellationToken ct = default);
    }
}
