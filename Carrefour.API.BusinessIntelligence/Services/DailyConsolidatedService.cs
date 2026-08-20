using Carrefour.API.BusinessIntelligence.DTOs;
using Carrefour.API.BusinessIntelligence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using System.Text.Json;

namespace Carrefour.API.BusinessIntelligence.Repositories
{
    public class DailyConsolidatedService : IDailyConsolidatedService
    {
        private readonly IDailyConsolidatedRepository _repository;
        private readonly IDistributedCache _cache;

        private const string CacheKey = "daily_consolidated_ledger";

        public DailyConsolidatedService(IDailyConsolidatedRepository repository, IDistributedCache cache)
        {
            _cache = cache;
            _repository = repository;
        }

        public async Task<IEnumerable<DailyConsolidatedDTO>> ReadAllAsync(
            CancellationToken ct = default
        )
        {
            string? cachedData = await _cache.GetStringAsync(CacheKey, ct);

            if (!string.IsNullOrEmpty(cachedData))
            {
                var cachedDailyConsolidatedDTO = JsonSerializer.Deserialize<IEnumerable<DailyConsolidatedDTO>>(cachedData);
                if (cachedDailyConsolidatedDTO != null)
                {
                    return cachedDailyConsolidatedDTO;
                }
            }

            var dailyConsolidatedList = await _repository.ReadAllAsync(ct);
            var dailyConsolidatedDTO = dailyConsolidatedList.Select(p => new DailyConsolidatedDTO(p));

            if (dailyConsolidatedDTO.Any())
            {
                string serializedData = JsonSerializer.Serialize(dailyConsolidatedDTO);

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                };

                await _cache.SetStringAsync(CacheKey, serializedData, cacheOptions, ct);
            }

            return dailyConsolidatedDTO;
        }
    }
}
