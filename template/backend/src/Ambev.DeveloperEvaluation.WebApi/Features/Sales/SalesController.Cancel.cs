using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public partial class SalesController
{
    /// <summary>
    /// Cancels an existing sale by its unique identifier.
    /// Once cancelled, the sale can no longer be updated or modified.
    /// </summary>
    /// <param name="id">The unique identifier of the sale (from route).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>200 OK with cancellation details if successful, 404 if not found, 400 if already cancelled.</returns>
    [HttpPatch("{id}/cancel")]
    [ProducesResponseType(typeof(CancelSaleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSale(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        // 1. Validate request
        var request = new CancelSaleRequest { Id = id };

        var validator = new CancelSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // 2. Send command to handler
        var command = new CancelSaleCommand(id);

        try
        {
            await _mediator.Send(command, cancellationToken);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (DomainException ex) // ← Catch already-cancelled scenario
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = ex.Message // "Sale is already cancelled."
            });
        }

        // 3. Return success response
        // Note: We don't have sale details here since handler returns Unit.Value
        // In a real scenario, you might want to modify the handler to return sale details
        return Ok(new CancelSaleResponse
        {
            Id = id,
            SaleNumber = "N/A", // ⚠️ Handler doesn't return this info
            Message = "Sale cancelled successfully."
        });
    }
}