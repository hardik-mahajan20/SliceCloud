using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
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
        string search, string status, string timeRange, DateTime? startDate, DateTime? endDate, string sortOrder = "asc", string sortColumn = "OrderDate", int page = 1, int pageSize = 10
    )
    {

        if (!startDate.HasValue || !endDate.HasValue)
        {
            DateTime today = DateTime.Today;

            switch (timeRange)
            {
                case "7":
                    startDate = today.AddDays(-7);
                    endDate = today;
                    break;
                case "30":
                    startDate = today.AddDays(-30);
                    endDate = today;
                    break;
                case "month":
                    startDate = new DateTime(today.Year, today.Month, 1);
                    endDate = today;
                    break;
                case "year":
                    startDate = new DateTime(today.Year, 1, 1);
                    endDate = today;
                    break;
            }
        }

        if (string.IsNullOrEmpty(sortColumn)) sortColumn = "OrderDate";
        if (string.IsNullOrEmpty(sortOrder)) sortOrder = "asc";


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
              => sortOrder == GeneralConstants.ASCENDING
                  ? query.OrderBy(o => o.Customer.CustomerName ?? string.Empty)
                  : query.OrderByDescending(o => o.Customer.CustomerName ?? string.Empty),
            OrderConstants.ORDER_DATE
              => sortOrder == GeneralConstants.ASCENDING
                  ? query.OrderBy(o => o.OrderDate).ThenBy(o => o.OrderId)
                  : query.OrderByDescending(o => o.OrderDate).ThenByDescending(o => o.OrderId),
            OrderConstants.TOTAL_AMOUNT
              => sortOrder == GeneralConstants.ASCENDING
                  ? query.OrderBy(o => o.TotalAmount).ThenBy(o => o.OrderId)
                  : query
                    .OrderByDescending(o => o.TotalAmount)
                    .ThenByDescending(o => o.OrderId),
            _
              => sortOrder == GeneralConstants.ASCENDING
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
                        CustomerName = o.Customer?.CustomerName ?? GeneralConstants.NA,
                        OrderDate = o.OrderDate ?? DateTime.Now,
                        TotalAmount = o.TotalAmount,
                        Rating = o.Rating ?? 0m,
                        PaymentMode = o.PaymentMode ?? GeneralConstants.NA,
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

    public async Task<List<Order>> GetFilteredCustomersAsync(string searchText, DateTime? startDate, DateTime? endDate, int? orderStatus, string sortColumn, string sortOrder)
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
                    sortOrder == GeneralConstants.ASCENDING
                        ? query.OrderBy(o => o.Customer.CustomerName)
                        : query.OrderByDescending(o => o.Customer.CustomerName);
                break;
            case OrderConstants.ORDER_DATE:
                query =
                    sortOrder == GeneralConstants.ASCENDING
                        ? query.OrderBy(o => o.OrderDate)
                        : query.OrderByDescending(o => o.OrderDate);
                break;
            case OrderConstants.TOTAL_AMOUNT:
                query =
                    sortOrder == GeneralConstants.ASCENDING
                        ? query.OrderBy(o => o.TotalAmount)
                        : query.OrderByDescending(o => o.TotalAmount);
                break;
            default:
                query =
                    sortOrder == GeneralConstants.ASCENDING
                        ? query.OrderBy(o => o.OrderId)
                        : query.OrderByDescending(o => o.OrderId);
                break;
        }
        return await query.ToListAsync();
    }

    #endregion

    public async Task<FileResult> ExportOrdersToExcel(string searchText, DateTime? startDate, DateTime? endDate, int? orderStatus, string sortColumn, string sortOrder, string webRootPath)
    {
        List<Order>? orders = await GetFilteredCustomersAsync(searchText, startDate, endDate, orderStatus, sortColumn, sortOrder);

        using XLWorkbook? workbook = new XLWorkbook();
        IXLWorksheet? worksheet = workbook.Worksheets.Add("Orders");

        string imagePath = Path.Combine(webRootPath, "images/logo.png");

        if (System.IO.File.Exists(imagePath))
        {
            IXLRange? mergedRange = worksheet.Range("O2:P6").Merge();
            mergedRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            mergedRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            double mergedWidth = worksheet.Columns("O:P").Sum(c => c.Width) * 7;
            double mergedHeight = worksheet.Rows(2, 6).Sum(r => r.Height);

            ClosedXML.Excel.Drawings.IXLPicture? picture = worksheet.AddPicture(imagePath)
                .MoveTo(worksheet.Cell("O2"))
                .WithSize((int)mergedWidth, (int)mergedHeight);
        }

        IXLRange? statusRange = worksheet.Range("A2:B3");
        statusRange.Merge().Value = "Status:";
        statusRange.Style.Font.Bold = true;
        statusRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
        statusRange.Style.Font.FontColor = XLColor.White;
        statusRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        statusRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        statusRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        string statusTextOrder = orderStatus switch
        {
            0 => "Pending",
            1 => "InProgress",
            2 => "Served",
            3 => "Completed",
            4 => "Cancelled",
            5 => "On Hold",
            6 => "Failed",
            _ => "All Status"
        };

        IXLRange? allStatusRange = worksheet.Range("C2:F3");
        allStatusRange.Merge().Value = statusTextOrder;
        allStatusRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        allStatusRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        allStatusRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        IXLRange? searchLabelRange = worksheet.Range("H2:I3");
        searchLabelRange.Merge().Value = "Search Text:";
        searchLabelRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
        searchLabelRange.Style.Font.FontColor = XLColor.White;
        searchLabelRange.Style.Font.Bold = true;
        searchLabelRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        searchLabelRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        searchLabelRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        IXLRange? searchValueRange = worksheet.Range("J2:M3");
        searchValueRange.Merge().Value = string.IsNullOrEmpty(searchText) ? "" : searchText;
        searchValueRange.Style.Fill.BackgroundColor = XLColor.White;
        searchValueRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        searchValueRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        searchValueRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        IXLRange? dateLabelRange = worksheet.Range("A5:B6");
        dateLabelRange.Merge().Value = "Date:";
        dateLabelRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
        dateLabelRange.Style.Font.FontColor = XLColor.White;
        dateLabelRange.Style.Font.Bold = true;
        dateLabelRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        dateLabelRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        dateLabelRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        IXLRange? dateValueRange = worksheet.Range("C5:F6");
        string dateValueText;
        if (startDate.HasValue && endDate.HasValue)
        {
            dateValueText = $"{startDate.Value:dd-MM-yyyy} to {endDate.Value:dd-MM-yyyy}";
        }
        else if (startDate.HasValue)
        {
            dateValueText = startDate.Value.ToString("dd-MM-yyyy");
        }
        else if (endDate.HasValue)
        {
            dateValueText = endDate.Value.ToString("dd-MM-yyyy");
        }
        else
        {
            dateValueText = "All Time";
        }

        dateValueRange.Merge().Value = dateValueText;

        dateValueRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        dateValueRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        dateValueRange.Style.Fill.BackgroundColor = XLColor.White;
        dateValueRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        IXLRange? recordsLabelRange = worksheet.Range("H5:I6");
        recordsLabelRange.Merge().Value = "No. Of Records:";
        recordsLabelRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
        recordsLabelRange.Style.Font.FontColor = XLColor.White;
        recordsLabelRange.Style.Font.Bold = true;
        recordsLabelRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        recordsLabelRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        recordsLabelRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        IXLRange? recordsValueRange = worksheet.Range("J5:M6");
        recordsValueRange.Merge().Value = orders.Count();
        recordsValueRange.Style.Fill.BackgroundColor = XLColor.White;
        recordsValueRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        recordsValueRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        recordsValueRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        IXLRange? summaryRange = worksheet.Range("A2:M6");
        summaryRange.Style.Font.Bold = true;
        summaryRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        summaryRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        summaryRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        worksheet.Range("A9:A9").Merge().Value = "#Order ID";
        worksheet.Range("B9:D9").Merge().Value = "Date";
        worksheet.Range("E9:G9").Merge().Value = "Customer Name";
        worksheet.Range("H9:J9").Merge().Value = "Status";
        worksheet.Range("K9:L9").Merge().Value = "Payment Mode";
        worksheet.Range("M9:N9").Merge().Value = "Rating";
        worksheet.Range("O9:P9").Merge().Value = "Total Amount";

        IXLRange? headerRange = worksheet.Range("A9:P9");
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        int row = 10;
        foreach (Order? order in orders)
        {
            worksheet.Cell(row, 1).Value = order.OrderId;
            worksheet.Range(row, 2, row, 4).Merge().Value = order.OrderDate?.ToString("dd-MM-yyyy HH:mm:ss") ?? "";
            worksheet.Range(row, 5, row, 7).Merge().Value = order.Customer.CustomerName;

            string statusText = order.Status switch
            {
                0 => "Pending",
                1 => "In-Progress",
                2 => "Served",
                3 => "Completed",
                4 => "Cancelled",
                5 => "On-Hold",
                6 => "Failed",
                _ => "Unknown"
            };
            worksheet.Range(row, 8, row, 10).Merge().Value = statusText;
            worksheet.Range(row, 11, row, 12).Merge().Value = order.PaymentMode;
            worksheet.Range(row, 13, row, 14).Merge().Value = order.Rating;
            worksheet.Range(row, 15, row, 16).Merge().Value = order.TotalAmount;

            IXLCell? statusCell = worksheet.Cell(row, 8);
            switch (order.Status)
            {
                case (int)OrderStatus.Pending:
                    break;
                case (int)OrderStatus.InProgress:
                    break;
                case (int)OrderStatus.Served:
                    break;
                case (int)OrderStatus.Completed:
                    break;
                case (int)OrderStatus.Cancelled:
                    break;
                case (int)OrderStatus.OnHold:
                    break;
                case (int)OrderStatus.Failed:
                    break;
                default:
                    break;
            }
            row++;
        }

        IXLRange? dataRange = worksheet.Range("A9:P" + (row - 1));
        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        worksheet.Columns().AdjustToContents();
        worksheet.Column(1).Width = 15;

        using MemoryStream? stream = new();
        workbook.SaveAs(stream);
        byte[]? fileBytes = stream.ToArray();
        return new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = $"Orders_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx"
        };
    }

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
                        TaxName = t.TaxId == 0 ? OrderConstants.OTHER : t.Tax?.TaxName ?? GeneralConstants.NA,
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
            CustomerName = order.Customer?.CustomerName ?? GeneralConstants.NA,
            CustomerPhone = order.Customer?.PhoneNo ?? GeneralConstants.NA,
            CustomerEmail = order.Customer?.Email ?? GeneralConstants.NA,
            NoOfPersons = order.NoOfPerson ?? 0,
            InvoiceNumber = order.InvoiceNumber ?? GeneralConstants.NA,
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
