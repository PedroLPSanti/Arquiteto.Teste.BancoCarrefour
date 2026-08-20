using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Carrefour.ETL.BusinessIntelligence.Workers;
using Carrefour.ETL.BusinessIntelligence.Models;
using Carrefour.ETL.BusinessIntelligence.Repositories;
using Carrefour.ETL.BusinessIntelligence.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContextFactory<Context>((sp, options) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    string dbHost = config["ConnectionStrings:dbHost"];
    string dbUser = config["ConnectionStrings:dbUser"];
    string dbPassword = config["ConnectionStrings:dbPassword"];
    string connectionString = $"Host={dbHost};Port=5432;Pooling=true;Database=db_carrefour;User Id={dbUser};Password={dbPassword};";
    options.UseNpgsql(connectionString);
});

builder.Services.AddTransient<IDailyConsolidatedRepository, DailyConsolidatedRepository>();
builder.Services.AddTransient<ILedgerActivityRepository, LedgerActivityRepository>();
builder.Services.AddScoped<ICreateDailyConsolidatedBusiness, CreateDailyConsolidatedBusiness>();

builder.Services.AddHostedService<Worker>();

try
{
    var host = builder.Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Starting process Error: {ex}");
}