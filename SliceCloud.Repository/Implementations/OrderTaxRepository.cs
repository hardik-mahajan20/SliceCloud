using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class OrderTaxRepository(SliceCloudContext sliceCloudContext) : IOrderTaxRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllOrderWithTaxesAsQueryable

    public IQueryable<OrderTaxMapping> GetAllOrderWithTaxesAsQueryable()
    {
        return _sliceCloudContext.OrderTaxMappings.Include(tm => tm.Tax).AsQueryable();
    }

    #endregion
    
}
