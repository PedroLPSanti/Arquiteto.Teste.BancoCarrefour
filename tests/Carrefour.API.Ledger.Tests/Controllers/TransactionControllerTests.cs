using Carrefour.API.Ledger.Controllers;
using Carrefour.API.Ledger.DTOs;
using Carrefour.API.Ledger.Enums;
using Carrefour.API.Ledger.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Carrefour.API.Ledger.Tests.Controllers;

public class TransactionControllerTests
{
    private readonly Mock<ILedgerActivityService> _serviceMock;
    private readonly TransactionController _sut;

    public TransactionControllerTests()
    {
        _serviceMock = new Mock<ILedgerActivityService>();
        _sut = new TransactionController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_WhenCalled_ReturnsOkWithListOfTransactions()
    {
        var expectedData = new List<LedgerActivityDTO>
        {
            new() { operation = OperationEnum.CREDIT, value = 150.00m, dateTimeInclusion = DateTime.UtcNow },
            new() { operation = OperationEnum.DEBIT, value = 50.25m, dateTimeInclusion = DateTime.UtcNow }
        };

        _serviceMock
            .Setup(s => s.ReadAllAsync(CancellationToken.None))
            .ReturnsAsync(expectedData);

        var actionResult = await _sut.GetAll(CancellationToken.None);

        var result = actionResult.Result as OkObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(200);

        var value = result.Value as IEnumerable<LedgerActivityDTO>;
        value.Should().BeEquivalentTo(expectedData);

        _serviceMock.Verify(s => s.ReadAllAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetAll_WhenNoTransactionsExist_ReturnsOkWithEmptyList()
    {
        _serviceMock
            .Setup(s => s.ReadAllAsync(CancellationToken.None))
            .ReturnsAsync(new List<LedgerActivityDTO>());

        var actionResult = await _sut.GetAll(CancellationToken.None);

        var result = actionResult.Result as OkObjectResult;
        result.Should().NotBeNull();
        (result!.Value as IEnumerable<LedgerActivityDTO>).Should().BeEmpty();
    }

    [Fact]
    public async Task Post_ValidDto_ReturnsOkWithCreatedTransaction()
    {
        var createDto = new CreateLedgerActivityDTO { operation = OperationEnum.CREDIT, value = 100.00m };
        var createdDto = new LedgerActivityDTO { operation = OperationEnum.CREDIT, value = 100.00m, dateTimeInclusion = DateTime.UtcNow };

        _serviceMock
            .Setup(s => s.CreateAsync(createDto, CancellationToken.None))
            .ReturnsAsync(createdDto);

        var actionResult = await _sut.Post(createDto, CancellationToken.None);

        var result = actionResult.Result as CreatedAtActionResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(201);

        var value = result.Value as LedgerActivityDTO;
        value.Should().BeEquivalentTo(createdDto);

        _serviceMock.Verify(s => s.CreateAsync(createDto, CancellationToken.None), Times.Once);
    }
}