using Carrefour.API.Ledger.Controllers;
using Carrefour.API.Ledger.DTOs;
using Carrefour.API.Ledger.Enums;
using Carrefour.API.Ledger.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Carrefour.API.Ledger.Tests.DTOs;

public class CreateLedgerActvityDTOTests
{
    private readonly Mock<ILedgerActivityService> _serviceMock;

    public CreateLedgerActvityDTOTests()
    {
        _serviceMock = new Mock<ILedgerActivityService>();
    }

    [Theory]
    [InlineData(0.00)]
    [InlineData(-0.01)]
    [InlineData(-100.50)]
    public void CreateLedgerActivityDTO_ValueZeroOrNegative_ThrowsArgumentOutOfRangeException(
        decimal invalidValue
    )
    {
        Action act = () =>
            new CreateLedgerActivityDTO { operation = OperationEnum.CREDIT, value = invalidValue };
        act.Should()
            .Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Value must be greater than or equal to 0.01.*");
    }
}
