using Carrefour.API.BusinessIntelligence.Models;
using Carrefour.API.BusinessIntelligence.Repositories;
using FluentAssertions;
using Moq;

namespace Carrefour.API.BusinessIntelligence.Tests.Services;

public class DailyConsolidatedServiceTests
{
    private readonly Mock<IDailyConsolidatedRepository> _repositoryMock;
    private readonly DailyConsolidatedService _sut;

    public DailyConsolidatedServiceTests()
    {
        _repositoryMock = new Mock<IDailyConsolidatedRepository>();
        _sut = new DailyConsolidatedService(_repositoryMock.Object);
    }

    [Fact]
    public async Task ReadAllAsync_WhenRepositoryReturnsItems_MapsAllToDtos()
    {
        var ct = CancellationToken.None;
        var models = new List<DailyConsolidated>
        {
            new()
            {
                valueCredit = 300.00m,
                valueDebit = 100.00m,
                debitQuantity = 1,
                creditQuantity = 2,
                consolidatedDate = new DateOnly(2026, 8, 20)
            },
            new()
            {
                valueCredit = 50.00m,
                valueDebit = 80.00m,
                debitQuantity = 3,
                creditQuantity = 1,
                consolidatedDate = new DateOnly(2026, 8, 21)
            }
        };

        _repositoryMock
            .Setup(r => r.ReadAllAsync(ct))
            .ReturnsAsync(models);

        var result = (await _sut.ReadAllAsync(ct)).ToList();

        result.Should().HaveCount(2);

        result[0].valueTotal.Should().Be(200.00m);
        result[0].valueCredit.Should().Be(300.00m);
        result[0].valueDebit.Should().Be(100.00m);
        result[0].consolidatedDate.Should().Be(new DateOnly(2026, 8, 20));

        result[1].valueTotal.Should().Be(-30.00m);
        result[1].valueCredit.Should().Be(50.00m);
        result[1].valueDebit.Should().Be(80.00m);

        _repositoryMock.Verify(r => r.ReadAllAsync(ct), Times.Once);
    }

    [Fact]
    public async Task ReadAllAsync_WhenRepositoryReturnsEmpty_ReturnsEmptyList()
    {
        var ct = CancellationToken.None;
        _repositoryMock
            .Setup(r => r.ReadAllAsync(ct))
            .ReturnsAsync(new List<DailyConsolidated>());

        var result = await _sut.ReadAllAsync(ct);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadAllAsync_PassesCancellationTokenToRepository()
    {
        var cts = new CancellationTokenSource();
        _repositoryMock
            .Setup(r => r.ReadAllAsync(cts.Token))
            .ReturnsAsync(new List<DailyConsolidated>());

        await _sut.ReadAllAsync(cts.Token);

        _repositoryMock.Verify(r => r.ReadAllAsync(cts.Token), Times.Once);
    }
}