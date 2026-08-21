using System.Text;
using System.Text.Json;
using Carrefour.API.BusinessIntelligence.DTOs;
using Carrefour.API.BusinessIntelligence.Repositories;
using Carrefour.API.BusinessIntelligence.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace Carrefour.API.BusinessIntelligence.Tests.Services;

public class CachedDailyConsolidatedServiceTests
{
    private readonly Mock<IDailyConsolidatedService> _innerServiceMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly CachedDailyConsolidatedService _sut;

    private const string CacheKey = "daily_consolidated_ledger";

    public CachedDailyConsolidatedServiceTests()
    {
        _innerServiceMock = new Mock<IDailyConsolidatedService>();
        _cacheMock = new Mock<IDistributedCache>();
        _sut = new CachedDailyConsolidatedService(_innerServiceMock.Object, _cacheMock.Object);
    }

    private void SetupCacheReturns(string? cachedJson)
    {
        byte[]? bytes = cachedJson is null ? null : Encoding.UTF8.GetBytes(cachedJson);

        _cacheMock
            .Setup(c => c.GetAsync(CacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bytes);
    }

    [Fact]
    public async Task ReadAllAsync_WhenCacheHasData_ReturnsCachedData_AndDoesNotCallInnerService()
    {
        var cachedDto = new List<DailyConsolidatedDTO>
        {
            new()
            {
                valueTotal = 200.00m,
                valueDebit = 100.00m,
                valueCredit = 300.00m,
                debitQuantity = 1,
                creditQuantity = 2,
                consolidatedDate = new DateOnly(2026, 8, 20)
            }
        };
        SetupCacheReturns(JsonSerializer.Serialize(cachedDto));

        var result = await _sut.ReadAllAsync(CancellationToken.None);

        result.Should().BeEquivalentTo(cachedDto);

        _innerServiceMock.Verify(s => s.ReadAllAsync(It.IsAny<CancellationToken>()), Times.Never);
        _cacheMock.Verify(c => c.SetAsync(
            It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ReadAllAsync_WhenCacheIsEmpty_CallsInnerService_AndReturnsItsData()
    {
        SetupCacheReturns(null);

        var innerData = new List<DailyConsolidatedDTO>
        {
            new()
            {
                valueTotal = 200.00m,
                valueDebit = 100.00m,
                valueCredit = 300.00m,
                debitQuantity = 1,
                creditQuantity = 2,
                consolidatedDate = new DateOnly(2026, 8, 20)
            }
        };
        _innerServiceMock
            .Setup(s => s.ReadAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(innerData);

        var result = await _sut.ReadAllAsync(CancellationToken.None);

        result.Should().BeEquivalentTo(innerData);
        _innerServiceMock.Verify(s => s.ReadAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReadAllAsync_WhenCacheIsEmpty_AndInnerServiceHasData_StoresResultInCache()
    {
        SetupCacheReturns(null);

        var innerData = new List<DailyConsolidatedDTO>
        {
            new()
            {
                valueTotal = 200.00m,
                valueDebit = 100.00m,
                valueCredit = 300.00m,
                debitQuantity = 1,
                creditQuantity = 2,
                consolidatedDate = new DateOnly(2026, 8, 20)
            }
        };
        _innerServiceMock
            .Setup(s => s.ReadAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(innerData);

        byte[]? capturedBytes = null;
        DistributedCacheEntryOptions? capturedOptions = null;

        _cacheMock
            .Setup(c => c.SetAsync(
                CacheKey,
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (_, bytes, options, _) =>
                {
                    capturedBytes = bytes;
                    capturedOptions = options;
                })
            .Returns(Task.CompletedTask);

        await _sut.ReadAllAsync(CancellationToken.None);

        capturedBytes.Should().NotBeNull();
        var deserialized = JsonSerializer.Deserialize<IEnumerable<DailyConsolidatedDTO>>(capturedBytes!);
        deserialized.Should().ContainSingle(d => d.valueTotal == 200.00m);

        capturedOptions.Should().NotBeNull();
        capturedOptions!.AbsoluteExpirationRelativeToNow.Should().Be(TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task ReadAllAsync_WhenCacheIsEmpty_AndInnerServiceReturnsEmptyList_DoesNotWriteToCache()
    {
        SetupCacheReturns(null);
        _innerServiceMock
            .Setup(s => s.ReadAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DailyConsolidatedDTO>());

        var result = await _sut.ReadAllAsync(CancellationToken.None);

        result.Should().BeEmpty();

        _cacheMock.Verify(c => c.SetAsync(
            It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ReadAllAsync_WhenCachedJsonDeserializesToNull_FallsBackToInnerService()
    {
        SetupCacheReturns("null");

        _innerServiceMock
            .Setup(s => s.ReadAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DailyConsolidatedDTO>());

        var result = await _sut.ReadAllAsync(CancellationToken.None);

        result.Should().NotBeNull();
        _innerServiceMock.Verify(s => s.ReadAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReadAllAsync_PassesCancellationTokenToCacheAndInnerService()
    {
        var cts = new CancellationTokenSource();

        _cacheMock
            .Setup(c => c.GetAsync(CacheKey, cts.Token))
            .ReturnsAsync((byte[]?)null);
        _innerServiceMock
            .Setup(s => s.ReadAllAsync(cts.Token))
            .ReturnsAsync(new List<DailyConsolidatedDTO>());

        await _sut.ReadAllAsync(cts.Token);

        _cacheMock.Verify(c => c.GetAsync(CacheKey, cts.Token), Times.Once);
        _innerServiceMock.Verify(s => s.ReadAllAsync(cts.Token), Times.Once);
    }
}