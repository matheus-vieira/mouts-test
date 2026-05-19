using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories.Sales;

/// <summary>
/// Handles update operations for existing <see cref="Sale"/> aggregates.
/// Uses upsert strategy for items: updates existing, inserts new, removes orphans.
/// </summary>
public class SaleUpdateRepository(DefaultContext context)
    : SaleRepository(context), ISaleUpdateRepository
{
    /// <inheritdoc/>
    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        // Load existing items from the database
        var existingItems = await _context.Set<SaleItem>()
            .Where(i => EF.Property<Guid>(i, "SaleId") == sale.Id)
            .ToListAsync(cancellationToken);

        var incomingIds = sale.Items.Select(i => i.Id).ToHashSet();
        var existingIds = existingItems.Select(i => i.Id).ToHashSet();

        // Remove orphans (items removed from the aggregate)
        var toRemove = existingItems.Where(i => !incomingIds.Contains(i.Id)).ToList();
        if (toRemove.Count > 0)
            _context.Set<SaleItem>().RemoveRange(toRemove);

        foreach (var incomingItem in sale.Items)
        {
            if (existingIds.Contains(incomingItem.Id))
            {
                // Update existing item
                _context.Entry(incomingItem).State = EntityState.Modified;
            }
            else
            {
                // Insert new item
                _context.Set<SaleItem>().Add(incomingItem);
            }
        }

        // Update Sale aggregate root scalar properties
        _context.Entry(sale).State = EntityState.Modified;

        await SaveChangesAsync(cancellationToken);
        return sale;
    }
}