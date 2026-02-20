using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IOrderRepository
{
    /// <summary>
    /// Retrieves all orders as an IQueryable for further filtering or querying.
    /// </summary>
    /// <returns>An IQueryable of all orders.</returns>
    IQueryable<Order> GetAllOrderAsQueryable();

    /// <summary>
    /// Retrieves an order along with its details by order ID asynchronously.
    /// </summary>
    /// <param name="orderId">The ID of the order to retrieve.</param>
    /// <returns>A task that returns the order with its details if found, otherwise null.</returns>
    Task<Order?> GetOrderWithDetailsAsync(int orderId);

    /// <summary>
    /// Retrieves a list of order items for a specific order as queryable.
    /// </summary>
    /// <returns>A list of order items as queryable.</returns>
    IQueryable<OrderedItem> GetOrderItemsDetailsAsQueryable(int orderId);
}
