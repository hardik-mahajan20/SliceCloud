using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;

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

    public async Task<List<OrderItemViewModel>> GetOrderItemsAsync(int orderId)
    {
        var orderItems = await _sliceCloudContext.OrderedItems
           .Where(oi => oi.OrderId == orderId)
           .Join(
               _sliceCloudContext.Items,
               oi => oi.ItemId,
               item => item.ItemId,
               (oi, item) =>
                   new
                   {
                       oi.OrderedItemId,
                       oi.ItemId,
                       oi.Quantity,
                       oi.OrderId,
                       ItemName = item.ItemName,
                       UnitPrice = item.Rate
                   }
           )
           .ToListAsync();

        List<int>? orderedItemIds = orderItems.Select(oi => oi.OrderedItemId).ToList();

        var orderedModifiers = await _sliceCloudContext.OrderedItemModifiers
            .Where(oim => orderedItemIds.Contains(oim.OrderedItemId))
            .Join(
                _sliceCloudContext.Modifiers,
                oim => oim.ItemModifierId,
                mod => mod.ModifierId,
                (oim, mod) =>
                    new
                    {
                        oim.OrderedItemId,
                        ModifierName = mod.ModifierName,
                        ModifierPrice = mod.Rate,
                        ModifierQuantity = oim.Quantity
                    }
            )
            .ToListAsync();

        List<OrderItemViewModel>? orderItemViewModels = orderItems
            .Select(
                oi =>
                {
                    List<ModifierViewModel>? itemModifiers = orderedModifiers
                        .Where(mod => mod.OrderedItemId == oi.OrderedItemId)
                        .Select(
                            mod =>
                                new ModifierViewModel
                                {
                                    ModifiedItemId = oi.ItemId,
                                    ModifierName = mod.ModifierName,
                                    Rate = mod.ModifierPrice,
                                    Quantity = mod.ModifierQuantity
                                }
                        )
                        .ToList();

                    decimal modifiersTotal = itemModifiers.Sum(
                        m => ((decimal?)m.Rate ?? 0) * (m.Quantity ?? 0)
                    );

                    return new OrderItemViewModel
                    {
                        ItemName = oi.ItemName,
                        ItemId = oi.ItemId,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        Total = oi.Quantity * oi.UnitPrice,
                        ModifierTotal = modifiersTotal,
                        Modifiers = itemModifiers
                    };
                }
            )
            .ToList();

        return orderItemViewModels;
    }

    #endregion
}
