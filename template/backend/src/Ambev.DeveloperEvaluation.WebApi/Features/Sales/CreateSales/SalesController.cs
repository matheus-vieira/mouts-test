using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

/// <summary>
/// Controller for managing sales transactions.
/// </summary>
/// <remarks>
/// This controller provides endpoints for creating, retrieving, updating, and deleting sales.
/// It uses MediatR to dispatch commands and queries to the application layer.
/// Inherits from <see cref="BaseController"/> for standardized response handling.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class SalesController(IMediator mediator, IMapper mapper) : BaseController
{
    private readonly IMediator _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Creates a new sale in the system.
    /// </summary>
    /// <param name="request">The sale creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created sale details (201 Created).</returns>
    /// <response code="201">Sale was successfully created.</response>
    /// <response code="400">Invalid request data (validation failed).</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale(
        [FromBody] CreateSaleRequest request, 
        CancellationToken cancellationToken)
    {
        var validator = new CreateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var command = _mapper.Map<CreateSaleCommand>(request);
        var result = await _mediator.Send(command, cancellationToken);
        var response = _mapper.Map<CreateSaleResponse>(result);

        return Created(string.Empty, new ApiResponseWithData<CreateSaleResponse>
        {
            Success = true,
            Message = "Sale created successfully",
            Data = response
        });
    }
}