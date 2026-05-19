using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.UpdateSale;

public class UpdateSaleHandlerTests
{
    private readonly ISaleReadRepository _readRepository;
    private readonly ISaleUpdateRepository _updateRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _readRepository = Substitute.For<ISaleReadRepository>();
        _updateRepository = Substitute.For<ISaleUpdateRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<UpdateSaleHandler>>();
        _handler = new UpdateSaleHandler(_readRepository, _updateRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given existing sale When handling Then updates and returns result")]
    public async Task Handle_ExistingSale_UpdatesSuccessfully()
    {
        // Arrange
        var sale = SaleEventTestData.GenerateValidSale();
        var command = UpdateSaleHandlerTestData.GenerateValidCommand(sale.Id);
        var expectedResult = new UpdateSaleResult { Id = sale.Id };

        _readRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<UpdateSaleResult>(sale).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        await _updateRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
        await _readRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existent sale When handling Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentSale_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        _readRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} was not found");
    }

    [Fact(DisplayName = "Given cancelled sale When handling Then domain rules should throw")]
    public async Task Handle_CancelledSale_ThrowsInvalidOperationException()
    {
        // Arrange
        var sale = SaleEventTestData.GenerateValidSale();
        sale.Cancel();
        var command = UpdateSaleHandlerTestData.GenerateValidCommand(sale.Id);

        _readRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Cannot update a cancelled sale.");
    }
}