using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class OrderTaxRepository(SliceCloudContext sliceCloudContext) : IOrderTaxRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetTaxMappingsByOrderId

    public async Task<List<OrderTaxMapping>> GetTaxMappingsByOrderIdAsync(int orderId)
    {
        return await _sliceCloudContext.OrderTaxMappings
                .Where(tm => tm.OrderId == orderId)
                .Include(tm => tm.Tax)
                .ToListAsync();
    }

    #endregion
}
