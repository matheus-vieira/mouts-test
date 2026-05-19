using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.DeleteSale;

public class DeleteSaleHandlerTests
{
    private readonly ISaleDeleteRepository _deleteRepository;
    private readonly DeleteSaleHandler _handler;

    public DeleteSaleHandlerTests()
    {
        _deleteRepository = Substitute.For<ISaleDeleteRepository>();
        _handler = new DeleteSaleHandler(_deleteRepository);
    }

    [Fact(DisplayName = "Valid command should delete sale and return true")]
    public async Task Given_ExistingSale_When_Handle_Then_ShouldReturnTrue()
    {
        // Arrange
        var command = new DeleteSaleCommand { Id = Guid.NewGuid() };
        _deleteRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        await _deleteRepository.Received(1).DeleteAsync(command.Id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Non-existent sale should return false")]
    public async Task Given_NonExistentSale_When_Handle_Then_ShouldReturnFalse()
    {
        // Arrange
        var command = new DeleteSaleCommand { Id = Guid.NewGuid() };
        _deleteRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        await _deleteRepository.Received(1).DeleteAsync(command.Id, Arg.Any<CancellationToken>());
    }
}