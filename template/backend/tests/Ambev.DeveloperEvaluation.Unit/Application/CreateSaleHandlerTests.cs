using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Unit tests for <see cref="CreateSaleHandler"/>.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _repository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _repository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CreateSaleHandler(_repository, _mapper, _mediator);
    }

    [Fact(DisplayName = "Given valid command When handling Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = CreateSaleHandlerTestData.GenerateSaleFromCommand(command);
        var result = new CreateSaleResult { Id = sale.Id, SaleNumber = sale.SaleNumber };

        _repository.CreateAsync(Arg.Any<DeveloperEvaluation.Domain.Entities.Sales.Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(result);

        // When
        var createSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        createSaleResult.Should().NotBeNull();
        createSaleResult.SaleNumber.Should().Be(command.SaleNumber);
        await _repository.Received(1).CreateAsync(
            Arg.Any<DeveloperEvaluation.Domain.Entities.Sales.Sale>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given valid command When handling Then publishes SaleCreatedEvent")]
    public async Task Handle_ValidRequest_PublishesSaleCreatedEvent()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = CreateSaleHandlerTestData.GenerateSaleFromCommand(command);

        _repository.CreateAsync(Arg.Any<DeveloperEvaluation.Domain.Entities.Sales.Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(new CreateSaleResult());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _mediator.Received(1).Publish(
            Arg.Any<SaleCreatedEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid command When handling Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CreateSaleCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
        await _repository.DidNotReceive().CreateAsync(
            Arg.Any<DeveloperEvaluation.Domain.Entities.Sales.Sale>(),
            Arg.Any<CancellationToken>());
    }
}