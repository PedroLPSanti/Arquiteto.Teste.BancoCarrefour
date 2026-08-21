using Carrefour.API.BusinessIntelligence.Controllers;
using Carrefour.API.BusinessIntelligence.DTOs;
using Carrefour.API.BusinessIntelligence.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Carrefour.API.BusinessIntelligence.Tests.Controllers;

public class DailyConsolidatedControllerTests
{
    private readonly Mock<IDailyConsolidatedService> _serviceMock;
    private readonly DailyConsolidatedController _sut;

    public DailyConsolidatedControllerTests()
    {
        _serviceMock = new Mock<IDailyConsolidatedService>();
        _sut = new DailyConsolidatedController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_WhenCalled_ReturnsOkWithListOfConsolidatedData()
    {
        // Arrange
        var ct = CancellationToken.None;
        var expectedData = new List<DailyConsolidatedDTO>
        {
            new()
            {
                valueTotal = 100.00m,
                valueDebit = 50.00m,
                valueCredit = 150.00m,
                debitQuantity = 2,
                creditQuantity = 3,
                consolidatedDate = new DateOnly(2026, 8, 20)
            },
            new()
            {
                valueTotal = 200.00m,
                valueDebit = 30.00m,
                valueCredit = 230.00m,
                debitQuantity = 1,
                creditQuantity = 4,
                consolidatedDate = new DateOnly(2026, 8, 21)
            }
        };

        _serviceMock
            .Setup(s => s.ReadAllAsync(ct))
            .ReturnsAsync(expectedData);

        // Act
        var actionResult = await _sut.GetAll(ct);

        // Assert
        var result = actionResult.Result as OkObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(200);

        var value = result.Value as IEnumerable<DailyConsolidatedDTO>;
        value.Should().BeEquivalentTo(expectedData);

        _serviceMock.Verify(s => s.ReadAllAsync(ct), Times.Once);
    }

    [Fact]
    public async Task GetAll_WhenNoDataExists_ReturnsOkWithEmptyList()
    {
        var ct = CancellationToken.None;

        _serviceMock
            .Setup(s => s.ReadAllAsync(ct))
            .ReturnsAsync(new List<DailyConsolidatedDTO>());

        var actionResult = await _sut.GetAll(ct);

        var result = actionResult.Result as OkObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(200);

        var value = result.Value as IEnumerable<DailyConsolidatedDTO>;
        value.Should().NotBeNull();
        value.Should().BeEmpty();
    }
}