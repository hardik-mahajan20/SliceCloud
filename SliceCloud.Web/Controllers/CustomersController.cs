using Microsoft.AspNetCore.Mvc;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Attributes;
using SliceCloud.Service.Interfaces;
using SliceCloud.Service.Utils;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the customer module related end points.
/// </summary>
public class CustomersController(ICustomerService customerService, IWebHostEnvironment webHostEnvironment) : Controller
{
    private readonly ICustomerService _customerService = customerService;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    #region Customers GET

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    public IActionResult Customers()
    {
        try
        {
            return View();
        }
        catch (Exception)
        {
            TempData.SetToast("error", "An error occurred while processing your request. Please try again.");
            return View();
        }
    }

    #endregion

    #region Customer's Partial View

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    public async Task<IActionResult> LoadCustomers(string search, string status, string timeRange, DateTime? startDate, DateTime? endDate, string sortOrder = "asc", string sortColumn = "CustomerName", int page = 1, int pageSize = 5)
    {
        try
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

            if (string.IsNullOrEmpty(sortColumn)) sortColumn = "CustomerName";
            if (string.IsNullOrEmpty(sortOrder)) sortOrder = "asc";

            ViewData["SortColumn"] = sortColumn;
            ViewData["SortDirection"] = sortOrder;

            var customers = await _customerService.GetPaginatedCustomersAsync(search, status, startDate, endDate, page, pageSize, sortColumn, sortOrder);

            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = customers.TotalItems;
            ViewBag.Page = page;
            ViewBag.TotalPages = customers.TotalPages;
            // ViewBag.FromRec = ((page - 1) * pageSize) + 1;
            // ViewBag.ToRec = Math.Min(page * pageSize, orders.TotalItems);
            if (customers.TotalItems == 0)
            {
                ViewBag.FromRec = 0;
                ViewBag.ToRec = 0;
            }
            else
            {
                ViewBag.FromRec = ((page - 1) * pageSize) + 1;
                ViewBag.ToRec = Math.Min(page * pageSize, customers.TotalItems);
            }
            return PartialView("_CustomersTablePartialView", customers);
        }
        catch (Exception)
        {
            TempData.SetToast("error", "An error occurred while processing your request. Please try again.");
            return PartialView("_CustomersTablePartialView", null);
        }
    }

    #endregion

    #region  GetCustomerHistory

    [HttpGet]
    public async Task<IActionResult> GetCustomerHistory(int id)
    {
        try
        {
            CustomerHistoryViewModel customerHistoryViewModel = await _customerService.GetCustomerHistoryAsync(id);

            if (customerHistoryViewModel == null)
            {
                return Json(new { success = false });
            }

            var result = new
            {
                success = true,
                data = new
                {
                    name = customerHistoryViewModel.Name,
                    phoneNumber = customerHistoryViewModel.PhoneNumber,
                    maxOrder = customerHistoryViewModel.MaxOrder,
                    avgBill = customerHistoryViewModel.AvgBill,
                    comingSince = customerHistoryViewModel.ComingSince.ToString("yyyy-MM-dd HH:mm:ss"),
                    visits = customerHistoryViewModel.Visits,
                    orders = customerHistoryViewModel.Orders.Select(o => new
                    {
                        orderDate = o.OrderDate.ToString("yyyy-MM-dd"),
                        PaymentMode = o.PaymentMode,
                        items = o.ItemsCount,
                        amount = o.TotalAmount,
                        orderType = o.OrderType
                    }).ToList()
                }
            };
            return Json(result);
        }
        catch (Exception)
        {
            TempData.SetToast("error", "An error occurred while processing your request. Please try again.");
            return Json(new { success = false });
        }
    }

    #endregion
}
