using System.Linq;
using Carrefour.API.BusinessIntelligence.DTOs;
using Carrefour.API.BusinessIntelligence.Models;
using Microsoft.EntityFrameworkCore;

namespace Carrefour.API.BusinessIntelligence.Repositories
{
    public class DailyConsolidatedService : IDailyConsolidatedService
    {
        private readonly IDailyConsolidatedRepository _repository;

        public DailyConsolidatedService(IDailyConsolidatedRepository repository) =>
            _repository = repository;

        public async Task<IEnumerable<DailyConsolidatedDTO>> ReadAllAsync(
            CancellationToken ct = default
        )
        {
            var products = await _repository.ReadAllAsync(ct);
            return products.Select(p => new DailyConsolidatedDTO(p));
        }
    }
}
