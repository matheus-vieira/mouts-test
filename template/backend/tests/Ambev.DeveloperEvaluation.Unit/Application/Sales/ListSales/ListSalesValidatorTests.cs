using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.ListSales;

public class ListSalesValidatorTests
{
    private readonly ListSalesValidator _validator;

    public ListSalesValidatorTests()
    {
        _validator = new ListSalesValidator();
    }

    [Theory(DisplayName = "Given invalid page or size When validating Then should have error")]
    [InlineData(0, 10)]   // Page 0 is invalid
    [InlineData(-1, 10)]  // Negative page
    [InlineData(1, 0)]    // Size 0 is invalid
    [InlineData(1, 101)]  // Size > 100 is invalid
    public void Given_InvalidPagination_When_Validating_Then_ShouldHaveError(int page, int size)
    {
        // Given
        var query = new ListSalesQuery { Page = page, PageSize = size };

        // When
        var result = _validator.TestValidate(query);

        // Then
        result.ShouldHaveAnyValidationError();
    }

    [Theory(DisplayName = "Given valid OrderBy When validating Then should not have error")]
    [InlineData("saleDate")]
    [InlineData("totalAmount desc")]
    [InlineData("customerName ASC")]
    [InlineData("branchName")]
    [InlineData("saleNumber desc")]
    [InlineData(null)]
    [InlineData("")]
    public void Given_ValidOrderBy_When_Validating_Then_ShouldNotHaveError(string? orderBy)
    {
        // Given
        var query = new ListSalesQuery { Page = 1, PageSize = 10, OrderBy = orderBy };

        // When
        var result = _validator.TestValidate(query);

        // Then
        result.ShouldNotHaveValidationErrorFor(x => x.OrderBy);
    }

    [Theory(DisplayName = "Given invalid OrderBy When validating Then should have error")]
    [InlineData("id")]                // Not in whitelist
    [InlineData("totalAmount foo")]   // Invalid direction
    [InlineData("saleDate asc desc")] // Too many parts
    [InlineData("; drop table sales")] // SQL injection attempt
    public void Given_InvalidOrderBy_When_Validating_Then_ShouldHaveError(string orderBy)
    {
        // Given
        var query = new ListSalesQuery { Page = 1, PageSize = 10, OrderBy = orderBy };

        // When
        var result = _validator.TestValidate(query);

        // Then
        result.ShouldHaveValidationErrorFor(x => x.OrderBy);
    }

    [Fact(DisplayName = "Given invalid date range When validating Then should have error")]
    public void Given_InvalidDateRange_When_Validating_Then_ShouldHaveError()
    {
        // Given
        var query = new ListSalesQuery
        {
            Page = 1,
            PageSize = 10,
            MinDate = DateTime.UtcNow,
            MaxDate = DateTime.UtcNow.AddDays(-1) // Max < Min
        };

        // When
        var result = _validator.TestValidate(query);

        // Then
        result.ShouldHaveValidationErrorFor(x => x.MinDate);
    }
}