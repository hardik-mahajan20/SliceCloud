using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IOrderTaxRepository
{
    /// <summary>
    /// Retrieves all orderTaxMappings with tax as queryable.
    /// </summary>
    /// <returns>All orderTaxMappings with tax as queryable.</returns>
    IQueryable<OrderTaxMapping> GetAllOrderWithTaxesAsQueryable();
}
