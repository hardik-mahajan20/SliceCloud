using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IOrderTaxRepository
{
    /// <summary>
    /// Retrieves all tax as queryable.
    /// </summary
    //  <returns>Returns all tax as queryable.</returns>
    IQueryable<OrderTaxMapping> GetAllOrderWithTaxesAsQueryable();
}
