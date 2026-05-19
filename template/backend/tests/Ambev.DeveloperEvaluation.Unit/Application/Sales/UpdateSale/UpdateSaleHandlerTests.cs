using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.UpdateSale;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _repository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<UpdateSaleHandler>>();
        _handler = new UpdateSaleHandler(_repository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given existing sale When updating items Then returns success response")]
    public async Task Handle_ExistingSale_ReturnsSuccessResponse()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new UpdateSaleCommand
        {
            Id = saleId,
            Items =
            [
                new UpdateSaleItemCommand { ProductId = Guid.NewGuid(), ProductName = "Product 1", Quantity = 5, UnitPrice = 10m }
            ]
        };

        var existingSale = Sale.Create("S001", DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch", 
            [SaleItem.Create(Guid.NewGuid(), "Old Item", 1, 10m)]);

        var expectedResult = new UpdateSaleResult { Id = saleId };

        _repository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(existingSale);
        _mapper.Map<UpdateSaleResult>(existingSale).Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        await _repository.Received(1).UpdateAsync(existingSale, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existing sale When updating Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingSale_ThrowsKeyNotFoundException()
    {
        // Given
        var command = new UpdateSaleCommand { Id = Guid.NewGuid(), Items = [] };
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given cancelled sale When updating Then throws InvalidOperationException")]
    public async Task Handle_CancelledSale_ThrowsInvalidOperationException()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new UpdateSaleCommand { Id = saleId, Items = [] };
        var existingSale = Sale.Create("S001", DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch", 
            [SaleItem.Create(Guid.NewGuid(), "Item", 1, 10m)]);
        
        existingSale.Cancel();

        _repository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(existingSale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot update a cancelled sale.");
    }
}