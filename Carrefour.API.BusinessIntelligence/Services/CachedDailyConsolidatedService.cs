using Carrefour.API.BusinessIntelligence.DTOs;
using Carrefour.API.BusinessIntelligence.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Carrefour.API.BusinessIntelligence.Services
{
    public class CachedDailyConsolidatedService : IDailyConsolidatedService
    {
        private readonly IDailyConsolidatedService _innerService;
        private readonly IDistributedCache _cache;
        private const string CacheKey = "daily_consolidated_ledger";

        public CachedDailyConsolidatedService(
            IDailyConsolidatedService innerService,
            IDistributedCache cache)
        {
            _innerService = innerService;
            _cache = cache;
        }

        public async Task<IEnumerable<DailyConsolidatedDTO>> ReadAllAsync(CancellationToken ct = default)
        {
            string? cachedData = await _cache.GetStringAsync(CacheKey, ct);

            if (!string.IsNullOrEmpty(cachedData))
            {
                var cachedDto = JsonSerializer.Deserialize<IEnumerable<DailyConsolidatedDTO>>(cachedData);
                if (cachedDto != null)
                {
                    return cachedDto;
                }
            }

            var data = await _innerService.ReadAllAsync(ct);

            if (data.Any())
            {
                string serializedData = JsonSerializer.Serialize(data);
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                };

                await _cache.SetStringAsync(CacheKey, serializedData, cacheOptions, ct);
            }

            return data;
        }
    }
}
