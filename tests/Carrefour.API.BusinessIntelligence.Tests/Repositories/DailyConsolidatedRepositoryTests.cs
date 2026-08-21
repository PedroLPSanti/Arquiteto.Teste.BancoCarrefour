using Carrefour.API.BusinessIntelligence.Models;
using Carrefour.API.BusinessIntelligence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Carrefour.API.BusinessIntelligence.Tests.Repositories;

public class DailyConsolidatedRepositoryTests
{
    private static Context GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test = full isolation
            .Options;

        return new Context(options);
    }

    [Fact]
    public async Task ReadAllAsync_WhenRecordsExist_ReturnsAllRecords()
    {
        // Arrange
        await using var context = GetInMemoryContext();
        context.DailyConsolidated.AddRange(
            new DailyConsolidated
            {
                valueCredit = 300.00m,
                valueDebit = 100.00m,
                debitQuantity = 1,
                creditQuantity = 2,
                consolidatedDate = new DateOnly(2026, 8, 20)
            },
            new DailyConsolidated
            {
                valueCredit = 50.00m,
                valueDebit = 80.00m,
                debitQuantity = 3,
                creditQuantity = 1,
                consolidatedDate = new DateOnly(2026, 8, 21)
            }
        );
        await context.SaveChangesAsync();

        var repository = new DailyConsolidatedRepository(context);

        // Act
        var result = (await repository.ReadAllAsync(CancellationToken.None)).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.consolidatedDate == new DateOnly(2026, 8, 20) && x.valueCredit == 300.00m);
        result.Should().Contain(x => x.consolidatedDate == new DateOnly(2026, 8, 21) && x.valueDebit == 80.00m);
    }

    [Fact]
    public async Task ReadAllAsync_WhenNoRecordsExist_ReturnsEmptyList()
    {
        await using var context = GetInMemoryContext();
        var repository = new DailyConsolidatedRepository(context);

        var result = await repository.ReadAllAsync(CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadAllAsync_ReturnsUntrackedEntities()
    {
        // proves AsNoTracking() is actually in effect - the change tracker
        // should have zero entries for DailyConsolidated after the read
        await using var context = GetInMemoryContext();
        context.DailyConsolidated.Add(new DailyConsolidated
        {
            valueCredit = 100.00m,
            valueDebit = 40.00m,
            debitQuantity = 1,
            creditQuantity = 1,
            consolidatedDate = new DateOnly(2026, 8, 21)
        });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear(); // reset tracker state after the seed write

        var repository = new DailyConsolidatedRepository(context);
        await repository.ReadAllAsync(CancellationToken.None);

        context.ChangeTracker.Entries<DailyConsolidated>().Should().BeEmpty();
    }
}