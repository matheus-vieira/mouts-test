using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.CreateSale;

public class CreateSaleCommandValidatorTests
{
    private readonly CreateSaleCommandValidator _validator;
    private readonly Faker _faker;

    public CreateSaleCommandValidatorTests()
    {
        _validator = new CreateSaleCommandValidator();
        _faker = new Faker("pt_BR");
    }

    private CreateSaleCommand BuildValidCommand() =>
        new()
        {
            SaleNumber = _faker.Random.AlphaNumeric(8).ToUpper(),
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = _faker.Person.FullName,
            BranchId = Guid.NewGuid(),
            BranchName = _faker.Company.CompanyName(),
            Items =
            [
                new CreateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = _faker.Commerce.ProductName(),
                    Quantity = 1,
                    UnitPrice = 10.0m
                }
            ]
        };

    [Fact(DisplayName = "Given valid command When validating Then passes validation")]
    public void Validate_ValidCommand_ShouldPass()
    {
        var result = _validator.Validate(BuildValidCommand());
        result.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Given empty SaleNumber When validating Then fails validation")]
    public void Validate_EmptySaleNumber_ShouldFail()
    {
        var command = BuildValidCommand();
        command.SaleNumber = string.Empty;

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SaleNumber");
    }

    [Fact(DisplayName = "Given default SaleDate When validating Then fails validation")]
    public void Validate_DefaultSaleDate_ShouldFail()
    {
        var command = BuildValidCommand();
        command.SaleDate = default;

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SaleDate");
    }

    [Fact(DisplayName = "Given empty CustomerId When validating Then fails validation")]
    public void Validate_EmptyCustomerId_ShouldFail()
    {
        var command = BuildValidCommand();
        command.CustomerId = Guid.Empty;

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CustomerId");
    }

    [Fact(DisplayName = "Given empty CustomerName When validating Then fails validation")]
    public void Validate_EmptyCustomerName_ShouldFail()
    {
        var command = BuildValidCommand();
        command.CustomerName = string.Empty;

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CustomerName");
    }

    [Fact(DisplayName = "Given empty BranchId When validating Then fails validation")]
    public void Validate_EmptyBranchId_ShouldFail()
    {
        var command = BuildValidCommand();
        command.BranchId = Guid.Empty;

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BranchId");
    }

    [Fact(DisplayName = "Given empty BranchName When validating Then fails validation")]
    public void Validate_EmptyBranchName_ShouldFail()
    {
        var command = BuildValidCommand();
        command.BranchName = string.Empty;

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BranchName");
    }

    [Fact(DisplayName = "Given empty Items When validating Then fails validation")]
    public void Validate_EmptyItems_ShouldFail()
    {
        var command = BuildValidCommand();
        command.Items = [];

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items");
    }

    [Fact(DisplayName = "Given item with zero Quantity When validating Then fails validation")]
    public void Validate_ItemQuantityZero_ShouldFail()
    {
        var command = BuildValidCommand();
        command.Items[0].Quantity = 0;

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact(DisplayName = "Given item with Quantity above 20 When validating Then fails validation")]
    public void Validate_ItemQuantityAbove20_ShouldFail()
    {
        var command = BuildValidCommand();
        command.Items[0].Quantity = 21;

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact(DisplayName = "Given item with zero UnitPrice When validating Then fails validation")]
    public void Validate_ItemUnitPriceZero_ShouldFail()
    {
        var command = BuildValidCommand();
        command.Items[0].UnitPrice = 0;

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact(DisplayName = "Given item with empty ProductId When validating Then fails validation")]
    public void Validate_ItemEmptyProductId_ShouldFail()
    {
        var command = BuildValidCommand();
        command.Items[0].ProductId = Guid.Empty;

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact(DisplayName = "Given item with empty ProductName When validating Then fails validation")]
    public void Validate_ItemEmptyProductName_ShouldFail()
    {
        var command = BuildValidCommand();
        command.Items[0].ProductName = string.Empty;

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}