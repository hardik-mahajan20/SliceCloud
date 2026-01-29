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
    public Task<PaginatedList<CustomerViewModel>> GetPaginatedCustomersAsync(string search, string status, DateTime? startDate, DateTime? endDate, int page, int pageSize, string sortColumn, string sortDirection);

    /// <summary>
    /// Retrieves the history of a customer by their ID.
    /// </summary>
    /// <param name="customerId">The ID of the customer to retrieve the history for.</param>
    /// <returns>A view model containing the customer's history.</returns>
    Task<CustomerHistoryViewModel> GetCustomerHistoryAsync(int customerId);
}
