using Carrefour.API.Ledger.DTOs;
using Carrefour.API.Ledger.Enums;
using Carrefour.API.Ledger.Models;
using Carrefour.API.Ledger.Repositories;
using Carrefour.API.Ledger.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Carrefour.API.Ledger.Tests.Services;

public class LedgerActivityServiceTests
{
    private readonly Mock<ILedgerActivityRepository> _repositoryMock;
    private readonly LedgerActivityService _sut;

    public LedgerActivityServiceTests()
    {
        _repositoryMock = new Mock<ILedgerActivityRepository>();
        _sut = new LedgerActivityService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_MapsToEntityAndReturnsDto()
    {
        // Arrange
        var ct = CancellationToken.None;
        var createDto = new CreateLedgerActivityDTO
        {
            operation = OperationEnum.CREDIT,
            value = 150.00m
        };

        LedgerActivity? capturedEntity = null;

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<LedgerActivity>(), ct))
            .Callback<LedgerActivity, CancellationToken>((entity, _) => capturedEntity = entity)
            .ReturnsAsync((LedgerActivity entity, CancellationToken _) => entity);

        var result = await _sut.CreateAsync(createDto, ct);

        capturedEntity.Should().NotBeNull();
        capturedEntity!.operation.Should().Be(createDto.operation);
        capturedEntity.value.Should().Be(createDto.value);

        result.Should().NotBeNull();
        result.operation.Should().Be(createDto.operation);
        result.value.Should().Be(createDto.value);

        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<LedgerActivity>(), ct), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_PassesCancellationTokenToRepository()
    {
        var cts = new CancellationTokenSource();
        var createDto = new CreateLedgerActivityDTO { operation = OperationEnum.DEBIT, value = 10.00m };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<LedgerActivity>(), cts.Token))
            .ReturnsAsync((LedgerActivity entity, CancellationToken _) => entity);

        await _sut.CreateAsync(createDto, cts.Token);

        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<LedgerActivity>(), cts.Token), Times.Once);
    }

    [Fact]
    public async Task ReadAllAsync_WhenRepositoryReturnsItems_MapsAllToDtos()
    {
        var ct = CancellationToken.None;
        var entities = new List<LedgerActivity>
        {
            new(new CreateLedgerActivityDTO { operation = OperationEnum.CREDIT, value = 100.00m }),
            new(new CreateLedgerActivityDTO { operation = OperationEnum.DEBIT, value = 25.50m })
        };

        _repositoryMock
            .Setup(r => r.ReadAllAsync(ct))
            .ReturnsAsync(entities);

        var result = (await _sut.ReadAllAsync(ct)).ToList();

        result.Should().HaveCount(2);
        result[0].operation.Should().Be(OperationEnum.CREDIT);
        result[0].value.Should().Be(100.00m);
        result[1].operation.Should().Be(OperationEnum.DEBIT);
        result[1].value.Should().Be(25.50m);

        _repositoryMock.Verify(r => r.ReadAllAsync(ct), Times.Once);
    }

    [Fact]
    public async Task ReadAllAsync_WhenRepositoryReturnsEmpty_ReturnsEmptyList()
    {
        var ct = CancellationToken.None;
        _repositoryMock
            .Setup(r => r.ReadAllAsync(ct))
            .ReturnsAsync(new List<LedgerActivity>());

        var result = await _sut.ReadAllAsync(ct);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}