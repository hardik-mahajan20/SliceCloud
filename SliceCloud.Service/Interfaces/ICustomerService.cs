using Microsoft.AspNetCore.Mvc;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Service.Interfaces;

public interface ICustomerService
{
    /// <summary>
    /// Retrieves a paginated list of customers based on search criteria, status, date range, and sorting options.
    /// </summary>
    /// <param name="search">The search term to filter customers.</param>
    /// <param name="status">The status of the customers to filter.</param>
    /// <param name="startDate">The start date for filtering customers by creation date.</param>
    /// <param name="endDate">The end date for filtering customers by creation date.</param>
    /// <param name="page">The page number for pagination.</param>
    /// <param name="pageSize">The number of customers per page.</param>
    /// <param name="sortColumn">The column to sort the results by.</param>
    /// <param name="sortDirection">The direction of sorting (e.g., ascending or descending).</param>
    /// <returns>A task that returns a paginated list of customer view models.</returns>
    public Task<PaginatedList<CustomerViewModel>> GetPaginatedCustomersAsync(string search, string status, string timeRange, DateTime? startDate, DateTime? endDate, string sortOrder = "asc", string sortColumn = "CustomerName", int page = 1, int pageSize = 5);

    /// <summary>
    /// Retrieves the history of a customer by their ID.
    /// </summary>
    /// <param name="customerId">The ID of the customer to retrieve the history for.</param>
    /// <returns>A view model containing the customer's history.</returns>
    Task<object> GetCustomerHistoryAsync(int customerId);

    /// <summary>
    /// Retrieves a filtered list of customers based on search text, date range, order status, and sorting options.
    /// </summary>
    /// <param name="searchText">The search term to filter customers.</param>
    /// <param name="startDate">The start date for filtering customers by creation date.</param>
    /// <param name="endDate">The end date for filtering customers by creation date.</param>
    /// <param name="orderStatus">The status of the orders to filter.</param>
    /// <param name="sortColumn">The column to sort the results by.</param>
    /// <param name="sortOrder">The direction of sorting (e.g., ascending or descending).</param>
    /// <returns>A collection of filtered customers.</returns>
    Task<IEnumerable<Customer>> GetFilteredOrders(
        string searchText,
        DateTime? startDate,
        DateTime? endDate,
        int? orderStatus,
        string sortColumn,
        string sortOrder);

    /// <summary>
    /// Exports customer data to an Excel file based on search criteria, date range, order status, and sorting options.
    /// </summary>
    /// <param name="searchText">The search term to filter customers.</param>
    /// <param name="startDate">The start date for filtering customers by creation date.</param>
    /// <param name="endDate">The end date for filtering customers by creation date.</param>
    /// <param name="orderStatus">The status of the orders to filter.</param>
    /// <param name="sortColumn">The column to sort the results by.</param>
    /// <param name="sortOrder">The direction of sorting (e.g., ascending or descending).</param>
    /// <param name="webRootPath">The root path of the web application for file storage.</param>
    /// <returns>A task that returns a file result containing the exported Excel file.</returns>
    Task<FileResult> ExportCustomersToExcel(string searchText, DateTime? startDate, DateTime? endDate, int? orderStatus, string sortColumn, string sortOrder, string webRootPath);
}
