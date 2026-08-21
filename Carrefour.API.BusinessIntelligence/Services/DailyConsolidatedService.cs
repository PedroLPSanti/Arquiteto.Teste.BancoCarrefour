using Carrefour.API.BusinessIntelligence.DTOs;

namespace Carrefour.API.BusinessIntelligence.Repositories
{
    public class DailyConsolidatedService : IDailyConsolidatedService
    {
        private readonly IDailyConsolidatedRepository _repository;

        public DailyConsolidatedService(IDailyConsolidatedRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DailyConsolidatedDTO>> ReadAllAsync(
            CancellationToken ct = default
        )
        {
            var dailyConsolidatedList = await _repository.ReadAllAsync(ct);
            var dailyConsolidatedDTO = dailyConsolidatedList.Select(p => new DailyConsolidatedDTO(p));

            return dailyConsolidatedDTO;
        }
    }
}
