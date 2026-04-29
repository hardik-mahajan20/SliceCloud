using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IOrderRepository
{
    /// <summary>
    /// Retrieves all orders as queryable.
    /// </summary>
    /// <returns>All orders as queryable.</returns>
    IQueryable<Order> GetAllOrderAsQueryable();

    /// <summary>
    /// Retrieves a order with details by its ID asynchronously.
    /// </summary>
    /// <param name="orderId">The ID of the order to retrieve.</param>
    /// <returns>A task that returns the order with details if found in the database, otherwise null.</returns>
    Task<Order?> GetOrderWithDetailsAsync(int orderId);

    /// <summary>
    /// Retrieves all orderedItem by its orderId as queryable.
    /// </summary>
    /// <param name="orderId">The ID of the orderedItem to retrieve.</param>
    /// <returns>A task that returns the orderedItem if found in the database, otherwise null.</returns>
    IQueryable<OrderedItem> GetOrderItemsDetailsAsQueryable(int orderId);
}
