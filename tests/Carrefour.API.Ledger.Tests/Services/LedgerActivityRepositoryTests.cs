using Carrefour.API.Ledger.DTOs;
using Carrefour.API.Ledger.Enums;
using Carrefour.API.Ledger.Models;
using Carrefour.API.Ledger.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Carrefour.API.Ledger.Tests.Repositories;

public class LedgerActivityRepositoryTests
{
    private static Context GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new Context(options);
    }

    [Fact]
    public async Task CreateAsync_PersistsEntity_AndReturnsIt()
    {
        await using var context = GetInMemoryContext();
        var repository = new LedgerActivityRepository(context);
        var entity = new LedgerActivity(new CreateLedgerActivityDTO
        {
            operation = OperationEnum.CREDIT,
            value = 100.00m
        });

        var result = await repository.CreateAsync(entity, CancellationToken.None);

        result.Should().NotBeNull();
        result.operation.Should().Be(OperationEnum.CREDIT);
        result.value.Should().Be(100.00m);

        var persisted = await context.ledgerActivity.AsNoTracking().ToListAsync();
        persisted.Should().ContainSingle();
        persisted[0].value.Should().Be(100.00m);
    }

    [Fact]
    public async Task CreateAsync_CalledTwice_PersistsBothEntities()
    {
        await using var context = GetInMemoryContext();
        var repository = new LedgerActivityRepository(context);

        await repository.CreateAsync(new LedgerActivity(new CreateLedgerActivityDTO
        {
            operation = OperationEnum.CREDIT,
            value = 50.00m
        }));
        await repository.CreateAsync(new LedgerActivity(new CreateLedgerActivityDTO
        {
            operation = OperationEnum.DEBIT,
            value = 30.00m
        }));

        var all = await context.ledgerActivity.ToListAsync();
        all.Should().HaveCount(2);
    }

    [Fact]
    public async Task ReadAllAsync_WhenEntitiesExist_ReturnsAllEntities()
    {
        await using var context = GetInMemoryContext();
        context.ledgerActivity.AddRange(
            new LedgerActivity(new CreateLedgerActivityDTO { operation = OperationEnum.CREDIT, value = 100.00m }),
            new LedgerActivity(new CreateLedgerActivityDTO { operation = OperationEnum.DEBIT, value = 20.00m })
        );
        await context.SaveChangesAsync();

        var repository = new LedgerActivityRepository(context);

        var result = (await repository.ReadAllAsync(CancellationToken.None)).ToList();

        result.Should().HaveCount(2);
        result.Should().Contain(x => x.value == 100.00m && x.operation == OperationEnum.CREDIT);
        result.Should().Contain(x => x.value == 20.00m && x.operation == OperationEnum.DEBIT);
    }

    [Fact]
    public async Task ReadAllAsync_WhenNoEntitiesExist_ReturnsEmptyList()
    {
        await using var context = GetInMemoryContext();
        var repository = new LedgerActivityRepository(context);

        var result = await repository.ReadAllAsync(CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadAllAsync_ReturnsUntrackedEntities()
    {
        await using var context = GetInMemoryContext();
        context.ledgerActivity.Add(new LedgerActivity(new CreateLedgerActivityDTO
        {
            operation = OperationEnum.CREDIT,
            value = 10.00m
        }));
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var repository = new LedgerActivityRepository(context);
        await repository.ReadAllAsync(CancellationToken.None);

        context.ChangeTracker.Entries<LedgerActivity>().Should().BeEmpty();
    }
}