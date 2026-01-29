using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Repository.Interfaces;

public interface ICustomerRepository
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
    /// <returns>A task that returns a paginated list of customers.</returns>
    public Task<PaginatedList<Customer>> GetPaginatedCustomersAsync(string search, string status, DateTime? startDate, DateTime? endDate, int page, int pageSize, string sortColumn, string sortDirection);

    /// <summary>
    /// Retrieves a customer along with their associated orders by customer ID.
    /// </summary>
    /// <param name="customerId">The ID of the customer to retrieve.</param>
    /// <returns>The customer with their orders if found, otherwise null.</returns>
    Task<Customer?> GetCustomerWithOrdersAsync(int customerId);

    /// <summary>
    /// Retrieves a customer by their customer ID.
    /// </summary>
    /// <param name="customerId">The ID of the customer.</param>
    /// <returns>The customer if found, otherwise null.</returns>
    Task<Customer?> GetCustomerByCustomerIdAsync(int customerId);

    /// <summary>
    /// Retrieves all orders associated with a specific customer ID.
    /// </summary>
    /// <param name="customerId">The ID of the customer.</param>
    /// <returns>A list of orders associated with the customer.</returns>
    Task<List<Order>> GetOrdersByCustomerIdAsync(int customerId);

    /// <summary>
    /// Adds a new customer to the database.
    /// </summary>
    /// <param name="customer">The customer to add.</param>
    /// <returns>The ID of the newly added customer.</returns>
    Task<int> AddCustomerAsync(Customer customer);

    /// <summary>
    /// Updates an existing customer's information.
    /// </summary>
    /// <param name="customer">The customer object with updated information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<bool> UpdateCustomerAsync(Customer customer);
}
