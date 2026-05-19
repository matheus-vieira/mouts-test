using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Handles the deletion of a sale.
/// </summary>
public class DeleteSaleHandler(ISaleDeleteRepository deleteRepository)
    : IRequestHandler<DeleteSaleCommand, bool>
{
    private readonly ISaleDeleteRepository _deleteRepository = deleteRepository;

    /// <inheritdoc/>
    public async Task<bool> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _deleteRepository.DeleteAsync(request.Id, cancellationToken);
        return deleted;
    }
}