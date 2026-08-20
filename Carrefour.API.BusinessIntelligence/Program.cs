using Carrefour.API.BusinessIntelligence.Models;
using Carrefour.API.BusinessIntelligence.Repositories;
using Carrefour.API.BusinessIntelligence.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<Context>(optionsBuilder =>
{
    string dbHost = builder.Configuration[$"ConnectionStrings:dbHost"];
    string dbUser = builder.Configuration[$"ConnectionStrings:dbUser"];
    string dbPassword = builder.Configuration[$"ConnectionStrings:dbPassword"];
    string connectionString =
        $"Host={dbHost};Port=5432;Pooling=true;Database=db_carrefour;User Id={dbUser};Password={dbPassword};";
    optionsBuilder.UseNpgsql(connectionString);
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    string redisHost = builder.Configuration[$"ConnectionStrings:redisHost"];
    string redisPassword = builder.Configuration[$"ConnectionStrings:redisPassword"];
    options.Configuration = $"{redisHost},password={redisPassword}";
    options.InstanceName = "CarrefourBI_"; // Optional prefix for keys
});

builder.Services.AddScoped<IDailyConsolidatedRepository, DailyConsolidatedRepository>();
builder.Services.AddScoped<IDailyConsolidatedService, DailyConsolidatedService>();
builder.Services.Decorate<IDailyConsolidatedService, CachedDailyConsolidatedService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<Context>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
