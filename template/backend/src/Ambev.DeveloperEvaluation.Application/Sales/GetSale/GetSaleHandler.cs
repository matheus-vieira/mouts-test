using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Handler for retrieving a sale by ID.
/// </summary>
public class GetSaleHandler(
    ISaleReadRepository repository,
    IMapper mapper) : IRequestHandler<GetSaleCommand, GetSaleResult>
{
    public async Task<GetSaleResult> Handle(GetSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} was not found");

        return mapper.Map<GetSaleResult>(sale);
    }
}