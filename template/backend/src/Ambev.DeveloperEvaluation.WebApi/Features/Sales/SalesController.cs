using Ambev.DeveloperEvaluation.WebApi.Common;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

/// <summary>
/// Controller for managing sales transactions.
/// This is a partial class. Each feature (Create, Get, Update, etc.) is handled in its own file.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public partial class SalesController(IMediator mediator, IMapper mapper) : BaseController
{
    private readonly IMediator _mediator = mediator;
    private readonly IMapper _mapper = mapper;
}