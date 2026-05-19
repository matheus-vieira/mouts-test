using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.GetSale;

public class GetSaleHandlerTests
{
    private readonly ISaleRepository _repository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _repository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_repository, _mapper);
    }

    [Fact(DisplayName = "Given existing sale When retrieving Then returns success response")]
    public async Task Handle_ExistingSale_ReturnsSuccessResponse()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new GetSaleCommand(saleId);
        var existingSale = Sale.Create(DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch", 
            [SaleItem.Create(Guid.NewGuid(), "Product", 2, 50m)]);

        var expectedResult = new GetSaleResult { Id = saleId, SaleNumber = "S001" };

        _repository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(existingSale);
        _mapper.Map<GetSaleResult>(existingSale).Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(saleId);
        await _repository.Received(1).GetByIdAsync(saleId, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existing sale When retrieving Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingSale_ThrowsKeyNotFoundException()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new GetSaleCommand(saleId);
        _repository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}