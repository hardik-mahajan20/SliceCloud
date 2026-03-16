using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class CustomerService(ICustomerRepository customerRepository) : ICustomerService
{
    private readonly ICustomerRepository _customerRepository = customerRepository;

    #region GetPaginatedCustomers

    public async Task<PaginatedList<CustomerViewModel>> GetPaginatedCustomersAsync(string search, string status, string timeRange, DateTime? startDate, DateTime? endDate, string sortOrder = "asc", string sortColumn = "CustomerName", int page = 1, int pageSize = 5)
    {

        if (!startDate.HasValue || !endDate.HasValue)
        {
            DateTime today = DateTime.Today;

            switch (timeRange)
            {
                case CustomerConstants.DATE_RANGE_7:
                    startDate = today.AddDays(-7);
                    endDate = today;
                    break;
                case CustomerConstants.DATE_RANGE_30:
                    startDate = today.AddDays(-30);
                    endDate = today;
                    break;
                case CustomerConstants.DATE_RANGE_MONTH:
                    startDate = new DateTime(today.Year, today.Month, 1);
                    endDate = today;
                    break;
                case CustomerConstants.DATE_RANGE_YEAR:
                    startDate = new DateTime(today.Year, 1, 1);
                    endDate = today;
                    break;
            }
        }

        if (string.IsNullOrEmpty(sortColumn)) sortColumn = CustomerConstants.CUSTOMER_NAME;
        if (string.IsNullOrEmpty(sortOrder)) sortOrder = GeneralConstants.ASCENDING;

        IQueryable<Customer>? query = _customerRepository.GetAllCustomersAsQueryable();

        DateTime? startUtc = startDate?.ToUniversalTime();
        DateTime? endUtc = endDate?.ToUniversalTime();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string trimmedSearch = search.Trim().ToLower();
            query = query.Where(
                o =>
                    (o.CustomerName != null && o.CustomerName.ToLower() == trimmedSearch)
                    || (o.Email != null && o.Email.ToLower() == trimmedSearch)
                    || (o.PhoneNo != null && o.PhoneNo.ToLower() == trimmedSearch)
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
            CustomerConstants.CREATE_DATE
              => sortOrder == GeneralConstants.ASCENDING
                  ? query.OrderBy(o => o.CreatedAt)
                  : query.OrderByDescending(o => o.CreatedAt),
            CustomerConstants.TOTAL_ORDER
               => sortOrder == GeneralConstants.ASCENDING
                   ? query.OrderBy(o => o.TotalOrder).ThenBy(o => o.CustomerId)
                   : query
                     .OrderByDescending(o => o.TotalOrder)
                     .ThenByDescending(o => o.CustomerId),
            CustomerConstants.CUSTOMER_NAME
              => sortOrder == GeneralConstants.ASCENDING
                  ? query.OrderBy(o => o.CustomerName)
                  : query.OrderByDescending(o => o.CustomerName),
            _ => query.OrderByDescending(o => o.CreatedAt)
        };

        PaginatedList<Customer>? customers = await PaginatedList<Customer>.CreateAsync(query, page, pageSize);

        List<CustomerViewModel>? customerViewModel = customers.Select(c => new CustomerViewModel
        {
            CustomerId = c.CustomerId,
            CustomerName = c.CustomerName,
            CreatedDate = c.CreatedAt ?? DateTime.Now,
            PhoneNumber = c.PhoneNo,
            Email = c.Email,
            TotalOrder = c.TotalOrder ?? 0,
        }).ToList();

        return new PaginatedList<CustomerViewModel>(customerViewModel, customers.TotalItems, page, pageSize);
    }

    #endregion

    #region GetCustomerHistory

    public async Task<object> GetCustomerHistoryAsync(int customerId)
    {
        Customer? customer = await _customerRepository.GetCustomerWithOrdersAsync(customerId);
        if (customer == null) return new { success = false, message = "Customer not found" };

        return new
        {
            success = true,
            data = new
            {
                name = customer.CustomerName,
                phoneNumber = customer.PhoneNo,
                maxOrder = customer.Orders.Count != 0 ? customer.Orders.Max(o => o.TotalAmount) : 0,
                avgBill = customer.Orders.Count != 0 ? Math.Round(customer.Orders.Average(o => o.TotalAmount), 2) : 0,
                comingSince = customer.CreatedAt ?? DateTime.Now,
                visits = customer.Orders.Count,
                orders = customer.Orders.Select(o => new OrderViewModel
                {
                    OrderDate = o.OrderDate ?? DateTime.Now,
                    OrderType = o.OrderType ?? GeneralConstants.NA,
                    PaymentMode = o.PaymentMode ?? GeneralConstants.NA,
                    ItemsCount = o.OrderedItems.Count,
                    TotalAmount = o.TotalAmount
                }).ToList()
            }
        };
    }

    #endregion

    #region  GetFilteredOrders

    public async Task<IEnumerable<Customer>> GetFilteredOrdersAsync(string searchText, DateTime? startDate, DateTime? endDate, int? orderStatus, string sortColumn, string sortOrder)
    {
        IQueryable<Customer>? query = _customerRepository.GetAllCustomersAsQueryable();

        DateTime? startUtc = startDate?.ToUniversalTime();

        DateTime? endUtc = endDate?.ToUniversalTime();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            string trimmedSearch = searchText.Trim().ToLower();
            query = query.Where(
                o =>
                    (o.CustomerName != null && o.CustomerName.ToLower() == trimmedSearch)
                    || (o.Email != null && o.Email.ToLower() == trimmedSearch)
            );
        }

        if (startUtc.HasValue)
        {
            query = query.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value >= startUtc.Value);
        }

        if (endUtc.HasValue)
        {
            DateTime endOfDay = endUtc.Value.Date.AddDays(1).AddTicks(-1); // End of day as max ticks on that date
            query = query.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value <= endOfDay);
        }

        switch (sortColumn)
        {
            case CustomerConstants.CUSTOMER_NAME:
                query = sortOrder == GeneralConstants.ASCENDING ? query.OrderBy(o => o.CustomerName) : query.OrderByDescending(o => o.CustomerName);
                break;
            case CustomerConstants.ORDER_DATE:
                query = sortOrder == GeneralConstants.ASCENDING ? query.OrderBy(o => o.CreatedAt) : query.OrderByDescending(o => o.CreatedAt);
                break;
            case CustomerConstants.TOTAL_AMOUNT:
                query = sortOrder == GeneralConstants.ASCENDING ? query.OrderBy(o => o.TotalOrder) : query.OrderByDescending(o => o.TotalOrder);
                break;
            default:
                query = sortOrder == GeneralConstants.ASCENDING ? query.OrderBy(o => o.CustomerId) : query.OrderByDescending(o => o.CustomerId);
                break;
        }

        Task<List<Customer>>? customers = query.ToListAsync();

        return await customers;
    }

    #endregion

    #region ExportCustomersToExcel

    public async Task<FileResult> ExportCustomersToExcelAsync(string searchText, DateTime? startDate, DateTime? endDate, int? orderStatus, string sortColumn, string sortOrder, string webRootPath)
    {
        IEnumerable<Customer>? customers = await GetFilteredOrdersAsync(searchText, startDate, endDate, orderStatus, sortColumn, sortOrder);

        using var workbook = new XLWorkbook();
        IXLWorksheet? worksheet = workbook.Worksheets.Add(CustomerConstants.CUSTOMERS);

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
        statusRange.Merge().Value = "Account:";
        statusRange.Style.Font.Bold = true;
        statusRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
        statusRange.Style.Font.FontColor = XLColor.White;
        statusRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        statusRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        statusRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        string statusTextOrder = orderStatus switch
        {
            0 => RolesConstants.MANAGER,
            1 => RolesConstants.CHEF,
            _ => RolesConstants.ADMIN
        };

        IXLRange allStatusRange = worksheet.Range("C2:F3");
        allStatusRange.Merge().Value = statusTextOrder;
        allStatusRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        allStatusRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        allStatusRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        IXLRange searchLabelRange = worksheet.Range("H2:I3");
        searchLabelRange.Merge().Value = "Search Text:";
        searchLabelRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
        searchLabelRange.Style.Font.FontColor = XLColor.White;
        searchLabelRange.Style.Font.Bold = true;
        searchLabelRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        searchLabelRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        searchLabelRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        IXLRange searchValueRange = worksheet.Range("J2:M3");
        searchValueRange.Merge().Value = string.IsNullOrEmpty(searchText) ? "" : searchText;
        searchValueRange.Style.Fill.BackgroundColor = XLColor.White;
        searchValueRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        searchValueRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        searchValueRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        IXLRange dateLabelRange = worksheet.Range("A5:B6");
        dateLabelRange.Merge().Value = "Date:";
        dateLabelRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
        dateLabelRange.Style.Font.FontColor = XLColor.White;
        dateLabelRange.Style.Font.Bold = true;
        dateLabelRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        dateLabelRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        dateLabelRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        IXLRange dateValueRange = worksheet.Range("C5:F6");
        string dateValueText;
        if (startDate.HasValue && endDate.HasValue)
            dateValueText = $"{startDate.Value:dd-MM-yyyy} to {endDate.Value:dd-MM-yyyy}";
        else if (startDate.HasValue)
            dateValueText = startDate.Value.ToString("dd-MM-yyyy");
        else if (endDate.HasValue)
            dateValueText = endDate.Value.ToString("dd-MM-yyyy");
        else
            dateValueText = "All Time";
        dateValueRange.Merge().Value = dateValueText;
        dateValueRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        dateValueRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        dateValueRange.Style.Fill.BackgroundColor = XLColor.White;
        dateValueRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        IXLRange recordsLabelRange = worksheet.Range("H5:I6");
        recordsLabelRange.Merge().Value = "No. Of Records:";
        recordsLabelRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
        recordsLabelRange.Style.Font.FontColor = XLColor.White;
        recordsLabelRange.Style.Font.Bold = true;
        recordsLabelRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        recordsLabelRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        recordsLabelRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        IXLRange recordsValueRange = worksheet.Range("J5:M6");
        recordsValueRange.Merge().Value = customers.Count();
        recordsValueRange.Style.Fill.BackgroundColor = XLColor.White;
        recordsValueRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        recordsValueRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        recordsValueRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        IXLRange summaryRange = worksheet.Range("A2:M6");
        summaryRange.Style.Font.Bold = true;
        summaryRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        summaryRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        summaryRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        worksheet.Range("A9:A9").Merge().Value = "Customer ID";
        worksheet.Range("B9:D9").Merge().Value = "Customer Name";
        worksheet.Range("E9:H9").Merge().Value = "Email";
        worksheet.Range("I9:K9").Merge().Value = "Date";
        worksheet.Range("L9:N9").Merge().Value = "Mobile Number";
        worksheet.Range("O9:P9").Merge().Value = "Total Order";

        IXLRange headerRange = worksheet.Range("A9:P9");
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        int row = 10;
        foreach (Customer customer in customers)
        {
            worksheet.Cell(row, 1).Value = customer.CustomerId;
            worksheet.Range(row, 2, row, 4).Merge().Value = customer.CustomerName;
            worksheet.Range(row, 5, row, 8).Merge().Value = customer.Email;
            worksheet.Range(row, 9, row, 11).Merge().Value = customer.CreatedAt?.ToString("dd-MM-yyyy HH:mm:ss") ?? "";
            worksheet.Range(row, 12, row, 14).Merge().Value = customer.PhoneNo;
            worksheet.Range(row, 15, row, 16).Merge().Value = customer.TotalOrder;
            row++;
        }

        IXLRange dataRange = worksheet.Range("A9:P" + (row - 1));
        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        worksheet.Columns().AdjustToContents();
        worksheet.Column(1).Width = 15;

        worksheet.Cells().Style.IncludeQuotePrefix = true;

        using (MemoryStream stream = new MemoryStream())
        {
            workbook.SaveAs(stream);
            byte[]? fileBytes = stream.ToArray();
            return new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = $"Orders_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx"
            };

        }
    }

    #endregion

}


