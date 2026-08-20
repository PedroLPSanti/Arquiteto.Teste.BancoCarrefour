using Carrefour.ETL.BusinessIntelligence.Models;
using Carrefour.ETL.BusinessIntelligence.Repositories;
using Carrefour.ETL.BusinessIntelligence.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Carrefour.ETL.BusinessIntelligence.Services
{
    public interface ICreateDailyConsolidatedBusiness
    {
        Task ProcessAsync(CancellationToken cancellationToken = default);
    }

    public class CreateDailyConsolidatedBusiness : ICreateDailyConsolidatedBusiness
    {
        private readonly IDailyConsolidatedRepository _dailyConsolidatedRepository;
        private readonly ILedgerActivityRepository _ledgerActivityRepository;
        private readonly ILogger _logger;
        public CreateDailyConsolidatedBusiness(
            ILogger<CreateDailyConsolidatedBusiness> logger,
            IDailyConsolidatedRepository dailyConsolidatedRepository,
            ILedgerActivityRepository ledgerActivityRepository
        )
        {
            _logger = logger;
            _dailyConsolidatedRepository = dailyConsolidatedRepository;
            _ledgerActivityRepository = ledgerActivityRepository;
        }

        public async Task ProcessAsync(CancellationToken ct = default)
        {
            try
            {
                long lastLedgerId = await _dailyConsolidatedRepository.GetLastProcessedLedgerIdAsync(ct);

                List<DailyConsolidated> incomingBatch = await _ledgerActivityRepository
                    .ExtractAndTransformIncrementalAsync(lastLedgerId, ct);

                if (!incomingBatch.Any()) return;

                var targetDates = incomingBatch.Select(x => x.consolidatedDate).Distinct();
                var existingRecords = await _dailyConsolidatedRepository.GetExistingRecordsByDatesAsync(targetDates, ct);

                foreach (var incoming in incomingBatch)
                {
                    if (existingRecords.TryGetValue(incoming.consolidatedDate, out var existing))
                    {
                        existing.valueDebit += incoming.valueDebit;
                        existing.valueCredit += incoming.valueCredit;
                        existing.debitQuantity += incoming.debitQuantity;
                        existing.creditQuantity += incoming.creditQuantity;
                        existing.idLastLedgerActivity = incoming.idLastLedgerActivity;
                    }
                    else {
                        await _dailyConsolidatedRepository.CreateAsync(incoming, ct);
                    }
                }

                await _dailyConsolidatedRepository.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                        ex,
                        "An unhandled exception occurred during the batch execution cycle."
                    );
            }
        }
    }
}
