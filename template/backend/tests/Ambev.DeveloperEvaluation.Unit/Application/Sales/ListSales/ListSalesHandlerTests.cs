using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Ambev.DeveloperEvaluation.Domain.Specifications.Sales;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.ListSales;

public class ListSalesHandlerTests
{
    private readonly ISaleReadRepository _readRepository;
    private readonly ListSalesHandler _handler;

    public ListSalesHandlerTests()
    {
        _readRepository = Substitute.For<ISaleReadRepository>();
        _handler = new ListSalesHandler(_readRepository);
    }

    private static Sale CreateSale() => Sale.Create(
        DateTime.UtcNow,
        Guid.NewGuid(),
        "Customer",
        Guid.NewGuid(),
        "Branch",
        [SaleItem.Create(Guid.NewGuid(), "Product", 1, 100m)]);

    [Fact(DisplayName = "Given valid query When handling Then returns paginated results")]
    public async Task Handle_ValidQuery_ReturnsPaginatedResults()
    {
        // Given
        var query = new ListSalesQuery { Page = 1, PageSize = 10 };
        var sales = new List<Sale> { CreateSale(), CreateSale() };
        var paginatedList = new PaginatedList<Sale>(sales, 2, 1, 10);

        _readRepository.GetPagedAsync(
            Arg.Any<SaleFilterSpecification>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>()
        ).Returns(paginatedList);

        // When
        var result = await _handler.Handle(query, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        await _readRepository.Received(1).GetPagedAsync(
            Arg.Is<SaleFilterSpecification>(s => s.CustomerName == null),
            1,
            10,
            null,
            Arg.Any<CancellationToken>());
    }

    [Theory(DisplayName = "Given query with OrderBy When handling Then normalizes property names")]
    [InlineData("totalAmount desc", "TotalAmount desc")]
    [InlineData("customerName", "CustomerName asc")]
    [InlineData("saleDate ASC", "SaleDate asc")]
    public async Task Handle_WithOrderBy_NormalizesPropertyNames(string inputOrder, string expectedNormalized)
    {
        // Given
        var query = new ListSalesQuery { Page = 1, PageSize = 10, OrderBy = inputOrder };
        var paginatedList = new PaginatedList<Sale>([], 0, 1, 10);

        _readRepository.GetPagedAsync(
            Arg.Any<SaleFilterSpecification>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>()
        ).Returns(paginatedList);

        // When
        await _handler.Handle(query, CancellationToken.None);

        // Then
        await _readRepository.Received(1).GetPagedAsync(
            Arg.Any<SaleFilterSpecification>(),
            1,
            10,
            expectedNormalized,
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given query with filters When handling Then passes filters to specification")]
    public async Task Handle_WithFilters_PassesFiltersToSpecification()
    {
        // Given
        var query = new ListSalesQuery
        {
            CustomerName = "Matheus",
            IsCancelled = true
        };
        var paginatedList = new PaginatedList<Sale>([], 0, 1, 10);

        _readRepository.GetPagedAsync(
            Arg.Any<SaleFilterSpecification>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>()
        ).Returns(paginatedList);

        // When
        await _handler.Handle(query, CancellationToken.None);

        // Then
        await _readRepository.Received(1).GetPagedAsync(
            Arg.Is<SaleFilterSpecification>(s =>
                s.CustomerName == "Matheus" &&
                s.IsCancelled == true),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }
}