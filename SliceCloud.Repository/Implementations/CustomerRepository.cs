using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Repository.Implementations;

public class CustomerRepository(SliceCloudContext sliceCloudContext) : ICustomerRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetPaginatedCustomers

    public async Task<PaginatedList<Customer>> GetPaginatedCustomersAsync(string search, string status, DateTime? startDate, DateTime? endDate, int page, int pageSize, string sortColumn, string sortDirection)
    {
        DateTime? startUtc = startDate?.ToUniversalTime();
        DateTime? endUtc = endDate?.ToUniversalTime();
        IQueryable<Customer>? query = _sliceCloudContext.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string trimmedSearch = search.Trim().ToLower();
            query = query.Where(
                o =>
                    (o.CustomerName != null && o.CustomerName.ToLower().Contains(trimmedSearch))
                    || (o.Email != null && o.Email.ToLower().Contains(trimmedSearch))
                    || (o.PhoneNo != null && o.PhoneNo.ToLower().Contains(trimmedSearch))
            );
        }

        if (startUtc.HasValue)
        {
            query = query.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value >= startUtc.Value);
        }

        if (endUtc.HasValue)
        {
            // End of day as max ticks on that date
            DateTime endOfDay = endUtc.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value <= endOfDay);
        }

        query = sortColumn switch
        {
            "CreateDate"
              => sortDirection == "asc"
                  ? query.OrderBy(o => o.CreatedAt)
                  : query.OrderByDescending(o => o.CreatedAt),
            "TotalOrder"
              => sortDirection == "asc"
                  ? query.OrderBy(o => o.TotalOrder).ThenBy(o => o.CustomerId)
                  : query
                    .OrderByDescending(o => o.TotalOrder)
                    .ThenByDescending(o => o.CustomerId),
            "CustomerName"
              => sortDirection == "asc"
                  ? query.OrderBy(o => o.CustomerName)
                  : query.OrderByDescending(o => o.CustomerName),
            _ => query.OrderByDescending(o => o.CreatedAt)
        };

        return await PaginatedList<Customer>.CreateAsync(query, page, pageSize);
    }

    #endregion

    #region GetCustomerWithOrders

    public async Task<Customer?> GetCustomerWithOrdersAsync(int customerId)
    {
        return await _sliceCloudContext.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.OrderedItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
    }

    #endregion

    #region GetCustomerByCustomerId

    public async Task<Customer?> GetCustomerByCustomerIdAsync(int customerId)
    {
        return await _sliceCloudContext.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
    }

    #endregion

    #region GetOrdersByCustomerId

    public async Task<List<Order>> GetOrdersByCustomerIdAsync(int customerId)
    {
        return await _sliceCloudContext.Orders.Where(o => o.CustomerId == customerId).ToListAsync();
    }

    #endregion

    #region AddCustomer
    public async Task<int> AddCustomerAsync(Customer customer)
    {
        _sliceCloudContext.Customers.Add(customer);
        await _sliceCloudContext.SaveChangesAsync();
        return customer.CustomerId;
    }

    #endregion

    #region UpdateCustomer

    public async Task<bool> UpdateCustomerAsync(Customer customer)
    {
        _sliceCloudContext.Customers.Update(customer);
        await _sliceCloudContext.SaveChangesAsync();
        return true;
    }

    #endregion
}
