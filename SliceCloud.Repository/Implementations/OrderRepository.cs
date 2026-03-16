using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class OrderRepository(SliceCloudContext sliceCloudContext) : IOrderRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllOrderAsQueryable

    public IQueryable<Order> GetAllOrderAsQueryable()
    {
        return _sliceCloudContext.Orders.Include(o => o.Customer).AsQueryable();
    }

    #endregion

    #region GetOrderWithDetails

    public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
    {
        Order? order = await _sliceCloudContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderedItems)
                .Include(o => o.Invoices)
                .Include(o => o.OrderTables)
                .ThenInclude(ot => ot.Table)
                .ThenInclude(t => t.Section)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

        return order;
    }

    #endregion

    #region GetOrderItemsAsync

    public IQueryable<OrderedItem> GetOrderItemsDetailsAsQueryable(int orderId)
    {
        return _sliceCloudContext.OrderedItems
            .Where(oi => oi.OrderId == orderId)
            .Include(oi => oi.Item)
            .Include(oi => oi.OrderedItemModifiers)
                .ThenInclude(oim => oim.ItemModifier)
            .AsQueryable();
    }

    #endregion

}
