namespace Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
/// <summary>
/// Composite repository contract for the <c>Sale</c> aggregate.
/// Combines all segregated interfaces for use in infrastructure registration.
/// </summary>
/// <remarks>
/// Handlers should depend on the specific interface they need:
/// - <see cref="ISaleCreateRepository"/> — CreateSaleHandler
/// - <see cref="ISaleReadRepository"/>   — GetSaleHandler, ListSalesHandler
/// - <see cref="ISaleUpdateRepository"/> — UpdateSaleHandler, CancelSaleHandler
/// - <see cref="ISaleDeleteRepository"/> — DeleteSaleHandler
/// </remarks>
public interface ISaleRepository :
    ISaleCreateRepository,
    ISaleReadRepository,
    ISaleUpdateRepository,
    ISaleDeleteRepository
{
}
