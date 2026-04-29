using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface ICustomerRepository
{
    /// <summary>
    /// Retrieves all customers as queryable.
    /// </summary>
    /// <returns>All customers as queryable.</returns>
    IQueryable<Customer> GetAllCustomersAsQueryable();

    /// <summary>
    /// Retrieves a customer along with their associated orders by customer ID.
    /// </summary>
    /// <param name="customerId">The ID of the customer to retrieve.</param>
    /// <returns>The customer with their orders if found, otherwise null.</returns>
    Task<Customer?> GetCustomerWithOrdersAsync(int customerId);
}
