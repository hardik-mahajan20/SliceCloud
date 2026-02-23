using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.Enums;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class OrderService(IOrderRepository orderRepository, IOrderTaxRepository orderTaxRepository) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IOrderTaxRepository _orderTaxRepository = orderTaxRepository;

    #region  GetOrders

    public async Task<PaginatedList<OrderViewModel>> GetOrdersAsync(
        string search,
        string status,
        DateTime? startDate,
        DateTime? endDate,
        int page,
        int pageSize,
        string sortColumn,
        string sortDirection
    )
    {
        IQueryable<Order>? query = _orderRepository.GetAllOrderAsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string? trimmedSearch = search.Trim().ToLower();
            query = query.Where(
                o =>
                    (
                        o.Customer != null
                        && o.Customer.CustomerName.ToLower() == trimmedSearch
                    )
                    ||
                    (
                        o.PaymentMode != null
                        && o.PaymentMode.ToLower() == trimmedSearch
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
            OrderConstants.CUSTOMER_NAME
              => sortDirection == GenralConstants.ASCENDING
                  ? query.OrderBy(o => o.Customer.CustomerName ?? string.Empty)
                  : query.OrderByDescending(o => o.Customer.CustomerName ?? string.Empty),
            OrderConstants.ORDER_DATE
              => sortDirection == GenralConstants.ASCENDING
                  ? query.OrderBy(o => o.OrderDate).ThenBy(o => o.OrderId)
                  : query.OrderByDescending(o => o.OrderDate).ThenByDescending(o => o.OrderId),
            OrderConstants.TOTAL_AMOUNT
              => sortDirection == GenralConstants.ASCENDING
                  ? query.OrderBy(o => o.TotalAmount).ThenBy(o => o.OrderId)
                  : query
                    .OrderByDescending(o => o.TotalAmount)
                    .ThenByDescending(o => o.OrderId),
            _
              => sortDirection == GenralConstants.ASCENDING
                  ? query.OrderBy(o => o.OrderId)
                  : query.OrderByDescending(o => o.OrderId),
        };

        PaginatedList<Order>? orders = await PaginatedList<Order>.CreateAsync(query, page, pageSize);

        List<OrderViewModel>? orderViewModels = orders
            .Select(
                o =>
                    new OrderViewModel
                    {
                        OrderId = o.OrderId,
                        CustomerName = o.Customer?.CustomerName ?? GenralConstants.NA,
                        OrderDate = o.OrderDate ?? DateTime.Now,
                        TotalAmount = o.TotalAmount,
                        Rating = o.Rating ?? 0m,
                        PaymentMode = o.PaymentMode ?? GenralConstants.NA,
                        Status = (OrderStatus)o.Status
                    }
            )
            .ToList();

        return new PaginatedList<OrderViewModel>(
            orderViewModels,
            orders.TotalItems,
            page,
            pageSize
        );
    }

    #endregion

    #region GetFilteredCustomers

    public List<Order> GetFilteredCustomersAsync(string searchText, DateTime? startDate, DateTime? endDate, int? orderStatus, string sortColumn, string sortOrder)
    {
        IQueryable<Order>? query = _orderRepository.GetAllOrderAsQueryable();

        if (!string.IsNullOrEmpty(searchText))
        {
            query = query.Where(
                o =>
                    EF.Functions.ILike(o.Customer.CustomerName, $"%{searchText}%")
                    || EF.Functions.ILike(o.OrderId.ToString(), $"%{searchText}%")
            );
        }

        if (startDate.HasValue)
        {
            query = query.Where(o => o.OrderDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(o => o.OrderDate <= endDate.Value);
        }

        if (orderStatus.HasValue)
        {
            query = query.Where(o => o.Status == orderStatus.Value);
        }
        switch (sortColumn)
        {
            case OrderConstants.CUSTOMER_NAME:
                query =
                    sortOrder == GenralConstants.ASCENDING
                        ? query.OrderBy(o => o.Customer.CustomerName)
                        : query.OrderByDescending(o => o.Customer.CustomerName);
                break;
            case OrderConstants.ORDER_DATE:
                query =
                    sortOrder == GenralConstants.ASCENDING
                        ? query.OrderBy(o => o.OrderDate)
                        : query.OrderByDescending(o => o.OrderDate);
                break;
            case OrderConstants.TOTAL_AMOUNT:
                query =
                    sortOrder == GenralConstants.ASCENDING
                        ? query.OrderBy(o => o.TotalAmount)
                        : query.OrderByDescending(o => o.TotalAmount);
                break;
            default:
                query =
                    sortOrder == GenralConstants.ASCENDING
                        ? query.OrderBy(o => o.OrderId)
                        : query.OrderByDescending(o => o.OrderId);
                break;
        }
        return query.ToList();
    }

    #endregion

    #region GetOrderInvoice

    public async Task<OrderInvoiceViewModel?> GetOrderInvoiceAsync(int orderId)
    {
        Order? order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
        if (order == null)
            return null;

        Invoice? invoice = order.Invoices.FirstOrDefault();
        OrderTable? orderTable = order.OrderTables.FirstOrDefault();

        List<OrderedItem>? orderedItems = await _orderRepository.GetOrderItemsDetailsAsQueryable(orderId).ToListAsync();

        List<OrderItemViewModel>? orderDetails = orderedItems.Select(oi =>
        {
            var itemModifiers = oi.OrderedItemModifiers.Select(oim =>
                new ModifierViewModel
                {
                    ModifiedItemId = oi.ItemId,
                    ModifierName = oim.ItemModifier.ModifierName,
                    Rate = oim.ItemModifier.Rate,
                    Quantity = oim.Quantity
                }).ToList();

            decimal modifiersTotal = itemModifiers.Sum(m =>
                ((decimal?)m.Rate ?? 0) * (m.Quantity ?? 0));

            return new OrderItemViewModel
            {
                ItemId = oi.ItemId,
                ItemName = oi.Item.ItemName,
                Quantity = oi.Quantity,
                UnitPrice = oi.Item.Rate,
                Total = oi.Quantity * oi.Item.Rate,
                ModifierTotal = modifiersTotal,
                Modifiers = itemModifiers
            };
        }).ToList();

        decimal subTotal =
            orderDetails.Sum(i => i.Total) + orderDetails.Sum(i => i.ModifierTotal);

        List<OrderTaxMapping>? taxMappings = await _orderTaxRepository.GetAllOrderWithTaxesAsQueryable().Where(tm => tm.OrderId == orderId).ToListAsync();

        List<TaxBreakdownViewModel>? taxBreakdown = taxMappings
            .Select(
                t =>
                    new TaxBreakdownViewModel
                    {
                        TaxName = t.TaxId == 0 ? OrderConstants.OTHER : t.Tax?.TaxName ?? GenralConstants.NA,
                        TaxValue = (decimal)(t.TaxValue ?? 0),
                    }
            )
            .ToList();

        decimal totalTax = taxBreakdown.Sum(t => t.TaxValue);
        decimal totalAmountDue = subTotal + totalTax;

        List<string?>? sections = order.OrderTables
            .Where(ot => ot.Table?.Section != null)
            .Select(ot => ot.Table?.Section?.SectionName)
            .Where(sectionName => sectionName != null)
            .Distinct()
            .ToList();

        if (sections == null)
            throw new Exception(OrderConstants.SECTIONS_ARE_EMPTY);

        List<string>? tables = order.OrderTables
            .Where(ot => ot.Table != null)
            .Select(ot => ot.Table.TableName)
            .Distinct()
            .ToList();

        return new OrderInvoiceViewModel
        {
            OrderId = order.OrderId,
            OrderStatus = ((OrderStatus)order.Status).ToString(),
            CustomerName = order.Customer?.CustomerName ?? GenralConstants.NA,
            CustomerPhone = order.Customer?.PhoneNo ?? GenralConstants.NA,
            CustomerEmail = order.Customer?.Email ?? GenralConstants.NA,
            NoOfPersons = order.NoOfPerson ?? 0,
            InvoiceNumber = order.InvoiceNumber ?? GenralConstants.NA,
            PaidOn = order.ModifiedAt,
            OrderDate = order.CreatedAt ?? DateTime.MinValue,
            ModifiedOn = order.ModifiedAt ?? DateTime.MinValue,
            OrderDuration = CalculateOrderDuration(order.CreatedAt, order.ModifiedAt),
            Sections = sections.Select(s => s ?? string.Empty).ToList(),
            Tables = tables,
            Items = orderDetails,
            SubTotal = subTotal,
            TotalAmountDue = totalAmountDue,
            TaxBreakdown = taxBreakdown, 
            PaymentMethod = order.PaymentMode ?? OrderConstants.PENDING
        };
    }

    #endregion

    #region Helpers

    private string CalculateOrderDuration(DateTime? orderDate, DateTime? modifiedOn)
    {
        if (orderDate.HasValue && modifiedOn.HasValue)
        {
            TimeSpan duration = modifiedOn.Value - orderDate.Value;
            int hours = (int)duration.TotalHours;
            int minutes = duration.Minutes;
            return $"{hours} Hours {minutes} Minutes";
        }
        return OrderConstants.ZERO_TIME;
    }

    #endregion
}
