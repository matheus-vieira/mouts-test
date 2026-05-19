using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using AutoMapper;
using Bogus;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.CreateSale;

public class CreateSaleHandlerTests
{
    private readonly ISaleCreateRepository _repository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly CreateSaleHandler _handler;
    private readonly Faker _faker;

    public CreateSaleHandlerTests()
    {
        _repository = Substitute.For<ISaleCreateRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CreateSaleHandler(_repository, _mapper, _mediator);
        _faker = new Faker("pt_BR");
    }

    private CreateSaleCommand BuildValidCommand(int itemCount = 1) =>
        new()
        {
            CustomerId = Guid.NewGuid(),
            CustomerName = _faker.Person.FullName,
            BranchId = Guid.NewGuid(),
            BranchName = _faker.Company.CompanyName(),
            Items = [.. Enumerable.Range(0, itemCount)
                .Select(_ => new CreateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = _faker.Commerce.ProductName(),
                    Quantity = _faker.Random.Int(1, 5),
                    UnitPrice = _faker.Random.Decimal(1, 100)
                })]
        };

    [Fact(DisplayName = "Given valid command When handling Then creates sale and returns result")]
    public async Task Handle_ValidCommand_ReturnsSaleResult()
    {
        // Given
        var command = BuildValidCommand();

        var createdSale = Sale.Create(
            DateTime.UtcNow,
            command.CustomerId,
            command.CustomerName,
            command.BranchId,
            command.BranchName,
            command.Items.Select(i => SaleItem.Create(
                i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)));

        var expectedResult = new CreateSaleResult
        {
            Id = createdSale.Id,
            SaleNumber = createdSale.SaleNumber,
            TotalAmount = createdSale.TotalAmount
        };

        _repository
            .CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);

        _mapper
            .Map<CreateSaleResult>(createdSale)
            .Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedResult.Id);
        result.SaleNumber.Should().Be(expectedResult.SaleNumber);
        result.TotalAmount.Should().Be(expectedResult.TotalAmount);

        await _repository.Received(1)
            .CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given valid command When handling Then publishes SaleCreatedEvent")]
    public async Task Handle_ValidCommand_PublishesSaleCreatedEvent()
    {
        // Given
        var command = BuildValidCommand();

        var createdSale = Sale.Create(
            DateTime.UtcNow,
            command.CustomerId,
            command.CustomerName,
            command.BranchId,
            command.BranchName,
            command.Items.Select(i => SaleItem.Create(
                i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)));

        _repository
            .CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);

        _mapper
            .Map<CreateSaleResult>(Arg.Any<Sale>())
            .Returns(new CreateSaleResult());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _mediator.Received(1)
            .Publish(
                Arg.Any<SaleCreatedEvent>(),
                Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid command When handling Then throws ValidationException")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Given
        var command = new CreateSaleCommand(); // empty — fails validation

        // When
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given valid command When handling Then maps sale to result")]
    public async Task Handle_ValidCommand_MapsSaleToResult()
    {
        // Given
        var command = BuildValidCommand();

        var createdSale = Sale.Create(
            DateTime.UtcNow,
            command.CustomerId,
            command.CustomerName,
            command.BranchId,
            command.BranchName,
            command.Items.Select(i => SaleItem.Create(
                i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)));

        _repository
            .CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);

        _mapper
            .Map<CreateSaleResult>(Arg.Any<Sale>())
            .Returns(new CreateSaleResult { Id = createdSale.Id });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<CreateSaleResult>(
            Arg.Is<Sale>(s => s.Id == createdSale.Id));
    }
}