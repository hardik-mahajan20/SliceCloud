using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;

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
    /// Retrieves a list of order items for a specific order asynchronously.
    /// </summary>
    /// <param name="orderId">The ID of the order to retrieve items for.</param>
    /// <returns>A task that returns a list of order items.</returns>
    Task<List<OrderItemViewModel>> GetOrderItemsAsync(int orderId);
}
