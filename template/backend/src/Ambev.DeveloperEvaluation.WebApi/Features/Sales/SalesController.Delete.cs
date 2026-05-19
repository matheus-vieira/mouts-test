using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public partial class SalesController
{
    /// <summary>
    /// Deletes an existing sale by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the sale (from route).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>204 No Content if deleted, 404 if not found.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSale(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new DeleteSaleRequest { Id = id };

        var validator = new DeleteSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var command = new DeleteSaleCommand { Id = id };
        var deleted = await _mediator.Send(command, cancellationToken);

        if (!deleted)
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Sale not found."
            });

        return NoContent();
    }
}