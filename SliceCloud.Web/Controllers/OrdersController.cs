using Microsoft.AspNetCore.Mvc;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Attributes;
using SliceCloud.Service.Interfaces;
using SliceCloud.Service.Utils;
using SliceCloud.Repository.Models;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the orders module related end points.
/// </summary>
public class OrdersController(IOrderService orderService, IWebHostEnvironment webHostEnvironment) : Controller
{

    private readonly IOrderService _orderService = orderService;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    #region Orders GET

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public IActionResult Orders()
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

    #region LoadOrders Partial View

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> LoadOrders(string search, string status, string timeRange, DateTime? startDate, DateTime? endDate, string sortOrder = "asc", string sortColumn = "OrderDate", int page = 1, int pageSize = 10)
    {
        try
        {
            ViewData["SortColumn"] = sortColumn;
            ViewData["SortDirection"] = sortOrder;

            PaginatedList<OrderViewModel> orders = await _orderService.GetOrdersAsync(search, status, timeRange, startDate, endDate, sortOrder = "asc", sortColumn = "OrderDate", page, pageSize);

            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = orders.TotalItems;
            ViewBag.Page = page;
            ViewBag.TotalPages = orders.TotalPages;
            if (orders.TotalItems == 0)
            {
                ViewBag.FromRec = 0;
                ViewBag.ToRec = 0;
            }
            else
            {
                ViewBag.FromRec = ((page - 1) * pageSize) + 1;
                ViewBag.ToRec = Math.Min(page * pageSize, orders.TotalItems);
            }

            return PartialView("_OrdersTablePartialView", orders);
        }
        catch (Exception)
        {
            TempData.SetToast("error", "An error occurred while processing your request. Please try again.");
            return PartialView("_OrdersTablePartialView", null);

        }
    }

    #endregion

    #region ExportOrders

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    public async Task<IActionResult> ExportOrders(string searchText, DateTime? startDate, DateTime? endDate, int? orderStatus, string sortColumn, string sortOrder)
    {
        try
        {
            IEnumerable<Order> orders = await _orderService.GetFilteredCustomersAsync(searchText, startDate, endDate, orderStatus, sortColumn, sortOrder);

            if (orders == null || !orders.Any())
            {
                Response.ContentType = "application/json";
                Response.StatusCode = 200;
                return Json(new { success = false, message = "No records found to download" });
            }

            FileResult fileResult = await _orderService.ExportOrdersToExcel(searchText, startDate, endDate, orderStatus, sortColumn, sortOrder, _webHostEnvironment.WebRootPath);

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

    #region DownloadInvoice

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    public async Task<IActionResult> DownloadInvoice(int orderId)
    {
        string webRootPath = _webHostEnvironment.WebRootPath;

        byte[] pdfBytes = await _orderService.ExportOrderPdf(
            webRootPath,
            orderId
        );

        return File(pdfBytes, "application/pdf", $"Invoice_{orderId}.pdf");
    }

    #endregion

    #region OrderDetails

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult?> OrderDetails(int orderId)
    {
        try
        {
            OrderInvoiceViewModel? orderDetails = await _orderService.GetOrderInvoiceAsync(orderId);
            if (orderDetails == null)
            {
                return null;
            }
            return View(orderDetails);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion

}
