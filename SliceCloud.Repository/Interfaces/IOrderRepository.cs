using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Repository.Interfaces;

public interface IOrderRepository
{
    /// <summary>
    /// Retrieves a paginated list of orders based on search criteria, status, date range, and sorting options.
    /// </summary>
    /// <param name="search">The search term to filter orders.</param>
    /// <param name="status">The status of the orders to filter.</param>
    /// <param name="startDate">The start date for filtering orders by creation date.</param>
    /// <param name="endDate">The end date for filtering orders by creation date.</param>
    /// <param name="page">The page number for pagination.</param>
    /// <param name="pageSize">The number of orders per page.</param>
    /// <param name="sortColumn">The column to sort the results by.</param>
    /// <param name="sortDirection">The direction of sorting (e.g., ascending or descending).</param>
    /// <returns>A task that returns a paginated list of orders.</returns>
    Task<PaginatedList<Order>> GetOrdersAsync(
        string search,
        string status,
        DateTime? startDate,
        DateTime? endDate,
        int page,
        int pageSize,
        string sortColumn,
        string sortDirection
    );

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
