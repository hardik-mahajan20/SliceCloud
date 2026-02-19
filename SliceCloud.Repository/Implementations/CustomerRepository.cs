using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class CustomerRepository(SliceCloudContext sliceCloudContext) : ICustomerRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllCustomersAsQuearyable

    public IQueryable<Customer> GetAllCustomersAsQuearyable()
    {
        return _sliceCloudContext.Customers.AsQueryable();
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
}
