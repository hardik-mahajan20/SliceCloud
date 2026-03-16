using Microsoft.AspNetCore.Mvc;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.Models;
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
            ViewData["SortColumn"] = sortColumn;
            ViewData["SortDirection"] = sortOrder;

            PaginatedList<CustomerViewModel>? customerViewModels = await _customerService.GetPaginatedCustomersAsync(search, status, timeRange, startDate, endDate, sortOrder, sortColumn, page, pageSize);

            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = customerViewModels.TotalItems;
            ViewBag.Page = page;
            ViewBag.TotalPages = customerViewModels.TotalPages;
            if (customerViewModels.TotalItems == 0)
            {
                ViewBag.FromRec = 0;
                ViewBag.ToRec = 0;
            }
            else
            {
                ViewBag.FromRec = ((page - 1) * pageSize) + 1;
                ViewBag.ToRec = Math.Min(page * pageSize, customerViewModels.TotalItems);
            }
            return PartialView("_CustomersTablePartialView", customerViewModels);
        }
        catch (Exception)
        {
            TempData.SetToast("error", "An error occurred while processing your request. Please try again.");
            return PartialView("_CustomersTablePartialView", null);
        }
    }

    #endregion

    #region  GetCustomerHistory

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> GetCustomerHistory(int id)
    {
        try
        {
            object? customer = await _customerService.GetCustomerHistoryAsync(id);

            if (customer == null)
            {
                return Json(new { success = false });
            }

            return Json(customer);
        }
        catch (Exception)
        {
            TempData.SetToast("error", "An error occurred while processing your request. Please try again.");
            return Json(new { success = false });
        }
    }

    #endregion

    #region ExportCustomers

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    public async Task<IActionResult> ExportCustomers(string searchText, DateTime? startDate, DateTime? endDate, int? orderStatus, string sortColumn, string sortOrder)
    {

        try
        {
            IEnumerable<Customer> customers = await _customerService.GetFilteredOrdersAsync(searchText, startDate, endDate, orderStatus, sortColumn, sortOrder);

            if (customers == null || !customers.Any())
            {
                Response.ContentType = "application/json";
                Response.StatusCode = 200;
                return Json(new { success = false, message = "No records found to download" });
            }

            FileResult fileResult = await _customerService.ExportCustomersToExcelAsync(searchText, startDate, endDate, orderStatus, sortColumn, sortOrder, _webHostEnvironment.WebRootPath);

            if (fileResult == null)
            {
                Response.ContentType = "application/json";
                Response.StatusCode = 200;
                return Json(new { success = false, message = "File generation failed." });
            }

            return fileResult;
        }
        catch (Exception ex)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = 200;
            return Json(new { success = false, message = "An error occurred while processing your request: " + ex.Message });
        }
    }

    #endregion

}
