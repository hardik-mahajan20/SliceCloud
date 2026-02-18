using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Service.Interfaces;

public interface IOrderService
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
    /// <returns>A task that returns a paginated list of order view models.</returns>
    Task<PaginatedList<OrderViewModel>> GetOrdersAsync(string search, string status, DateTime? startDate, DateTime? endDate, int page, int pageSize, string sortColumn, string sortDirection);

    /// <summary>
    /// Retrieves a filtered list of customers based on search text, date range, order status, and sorting options.
    /// </summary>
    /// <param name="searchText">The search term to filter customers.</param>
    /// <param name="startDate">The start date for filtering customers by creation date.</param>
    /// <param name="endDate">The end date for filtering customers by creation date.</param>
    /// <param name="orderStatus">The status of the orders to filter.</param>
    /// <param name="sortColumn">The column to sort the results by.</param>
    /// <param name="sortOrder">The direction of sorting (e.g., ascending or descending).</param>
    /// <returns>A collection of filtered orders.</returns>
    List<Order> GetFilteredCustomersAsync(string searchText, DateTime? startDate, DateTime? endDate, int? orderStatus, string sortColumn, string sortOrder);

    /// <summary>
    /// Retrieves the invoice details for a specific order asynchronously.
    /// </summary>
    /// <param name="orderId">The ID of the order to retrieve the invoice for.</param>
    /// <returns>A task that returns the order invoice view model.</returns>
    Task<OrderInvoiceViewModel?> GetOrderInvoiceAsync(int orderId);
}
