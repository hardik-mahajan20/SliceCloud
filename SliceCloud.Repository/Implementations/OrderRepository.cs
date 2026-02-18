using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Repository.Implementations;

public class OrderRepository(SliceCloudContext sliceCloudContext) : IOrderRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    public async Task<PaginatedList<Order>> GetOrdersAsync(string search, string status, DateTime? startDate, DateTime? endDate, int page, int pageSize, string sortColumn, string sortDirection)
    {
        IQueryable<Order> query = _sliceCloudContext.Orders.Include(o => o.Customer).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string? trimmedSearch = search.Trim().ToLower();
            query = query.Where(
                o =>
                    (
                        o.Customer != null
                        && o.Customer.CustomerName.ToLower().Contains(trimmedSearch)
                    )
                    || o.OrderId.ToString().Contains(trimmedSearch)
                    || (
                        o.PaymentMode != null && o.PaymentMode.ToLower().Contains(trimmedSearch)
                    )
            );
        }

        if (!string.IsNullOrEmpty(status) && int.TryParse(status, out int statusValue))
        {
            query = query.Where(o => o.Status == statusValue);
        }

        if (startDate.HasValue)
        {
            query = query.Where(
                o => o.OrderDate.HasValue && o.OrderDate.Value.Date >= startDate.Value.Date
            );
        }
        if (endDate.HasValue)
        {
            DateTime endOfDay = endDate.Value.Date.AddDays(1).AddSeconds(-1);
            query = query.Where(o => o.OrderDate.HasValue && o.OrderDate.Value <= endOfDay);
        }

        query = sortColumn switch
        {
            "CustomerName"
              => sortDirection == "asc"
                  ? query.OrderBy(o => o.Customer.CustomerName ?? string.Empty)
                  : query.OrderByDescending(o => o.Customer.CustomerName ?? string.Empty),
            "OrderDate"
              => sortDirection == "asc"
                  ? query.OrderBy(o => o.OrderDate).ThenBy(o => o.OrderId)
                  : query.OrderByDescending(o => o.OrderDate).ThenByDescending(o => o.OrderId),
            "TotalAmount"
              => sortDirection == "asc"
                  ? query.OrderBy(o => o.TotalAmount).ThenBy(o => o.OrderId)
                  : query
                    .OrderByDescending(o => o.TotalAmount)
                    .ThenByDescending(o => o.OrderId),
            _
              => sortDirection == "asc"
                  ? query.OrderBy(o => o.OrderId)
                  : query.OrderByDescending(o => o.OrderId),
        };

        return await PaginatedList<Order>.CreateAsync(query, page, pageSize);
    }

    #region GetAllOrderAsQueryable

    public IQueryable<Order> GetAllOrderAsQueryable()
    {
        return _sliceCloudContext.Orders.Include(o => o.Customer).AsNoTracking();
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

        if (order == null)
        {
            throw new ArgumentNullException(nameof(order), "Order object is null.");
        }
        else
        {
            if (order.OrderTables.Any())
            {
                OrderTable? orderTable = order.OrderTables.FirstOrDefault();
            }
        }
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
