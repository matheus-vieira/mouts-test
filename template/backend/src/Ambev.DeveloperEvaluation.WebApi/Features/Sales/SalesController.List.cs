using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public partial class SalesController
{
    /// <summary>
    /// Returns a paged list of sales based on search criteria.
    /// </summary>
    /// <param name="request">Filtering, ordering, and pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of sales.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<ListSalesResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListSales(
        [FromQuery] ListSalesRequest request,
        CancellationToken cancellationToken)
    {
        var validator = new ListSalesRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var query = new ListSalesQuery
        {
            Page = request._page,
            PageSize = request._size,
            OrderBy = request._order,
            CustomerName = request.CustomerName,
            BranchName = request.BranchName,
            MinDate = request.MinDate,
            MaxDate = request.MaxDate,
            MinAmount = request.MinAmount,
            MaxAmount = request.MaxAmount,
            IsCancelled = request.IsCancelled
        };

        var result = await _mediator.Send(query, cancellationToken);

        return OkPaginated(result);
    }
}