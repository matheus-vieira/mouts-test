using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handles the <see cref="CreateSaleCommand"/> use case.
/// </summary>
/// <remarks>
/// Orchestration flow:
/// 1. Validate the incoming command via <see cref="CreateSaleCommandValidator"/>
/// 2. Build domain items using <see cref="SaleItem.Create"/> — discount is applied automatically
/// 3. Build the sale aggregate using <see cref="Sale.Create"/> — total is calculated automatically
/// 4. Persist the aggregate via <see cref="ISaleCreateRepository"/>
/// 5. Map the persisted aggregate to <see cref="CreateSaleResult"/> and return
///
/// This handler depends only on <see cref="ISaleCreateRepository"/> (write side),
/// following the interface segregation defined in the domain layer.
/// </remarks>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleCreateRepository _repository;
    private readonly IMapper _mapper;

    /// <param name="repository">Write-side repository for sale persistence.</param>
    /// <param name="mapper">AutoMapper instance for entity-to-DTO mapping.</param>
    public CreateSaleHandler(ISaleCreateRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>
    /// Processes the <see cref="CreateSaleCommand"/> and returns the created sale result.
    /// </summary>
    /// <param name="command">The command carrying all sale creation data.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A <see cref="CreateSaleResult"/> with the persisted sale data.</returns>
    /// <exception cref="ValidationException">Thrown when command validation fails.</exception>
    public async Task<CreateSaleResult> Handle(
        CreateSaleCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // Build domain items — SaleItem.Create enforces quantity limits and applies discount
        var domainItems = command.Items
            .Select(i => SaleItem.Create(
                i.ProductId,
                i.ProductName,
                i.Quantity,
                i.UnitPrice));

        // Build aggregate — Sale.Create enforces non-empty items and calculates total
        var sale = Sale.Create(
            command.SaleNumber,
            command.SaleDate,
            command.CustomerId,
            command.CustomerName,
            command.BranchId,
            command.BranchName,
            domainItems);

        var createdSale = await _repository.CreateAsync(sale, cancellationToken);

        return _mapper.Map<CreateSaleResult>(createdSale);
    }
}