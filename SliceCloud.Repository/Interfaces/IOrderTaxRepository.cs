using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IOrderTaxRepository
{
    /// <summary>
    /// Retrieves all tax mappings associated with a specific order ID asynchronously.
    /// </summary>
    /// <param name="orderId">The ID of the order whose tax mappings are to be retrieved.</param>
    /// <returns>A task that returns a list of tax mappings for the specified order.</returns>
    Task<List<OrderTaxMapping>> GetTaxMappingsByOrderIdAsync(int orderId);
}
