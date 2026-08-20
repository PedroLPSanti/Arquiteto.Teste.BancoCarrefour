using Carrefour.ETL.BusinessIntelligence.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Carrefour.ETL.BusinessIntelligence.Workers
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;

        public Worker(
            ILogger<Worker> logger,
            IServiceScopeFactory serviceScopeFactory,
            IConfiguration configuration
        )
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int workerInterval = _configuration.GetValue<int>("workerInterval", 60000);

            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(workerInterval));

            do
            {
                _logger.LogInformation("Running at: {time}", DateTimeOffset.Now);

                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var batchBusiness =
                            scope.ServiceProvider.GetRequiredService<ICreateDailyConsolidatedBusiness>();

                        await batchBusiness.ProcessAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "An unhandled exception occurred during the batch execution cycle."
                    );
                }
            } while (await timer.WaitForNextTickAsync(stoppingToken));
        }
    }
}
