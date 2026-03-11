using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Attributes;
using SliceCloud.Service.Interfaces;
using SliceCloud.Service.Utils;

using System.Text;
using iTextSharp.text;
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
        catch (Exception f)
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
        OrderInvoiceViewModel? invoiceModel = await _orderService.GetOrderInvoiceAsync(orderId);
        if (invoiceModel == null) return Json(new { success = false, message = "Invoice model not found" });

        using (MemoryStream memoryStream = new MemoryStream())
        {
            Document document = new Document(PageSize.A4, 30, 30, 40, 40);
            PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            BaseColor blueColor = new BaseColor(0, 102, 167);
            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, blueColor);
            Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            Font totalFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);

            string logoPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "logos", "logo.png");

            PdfPTable headerTable = new PdfPTable(2)
            {
                WidthPercentage = 50,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            headerTable.SetWidths(new float[] { 1f, 2f });
            headerTable.DefaultCell.Border = Rectangle.NO_BORDER;

            if (System.IO.File.Exists(logoPath))
            {
                Image logo = Image.GetInstance(logoPath);
                logo.ScaleToFit(60f, 60f);

                PdfPCell logoCell = new PdfPCell(logo)
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                headerTable.AddCell(logoCell);
            }

            PdfPCell titleCell = new PdfPCell(new Phrase("SliceCloud", titleFont))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            headerTable.AddCell(titleCell);

            document.Add(headerTable);
            document.Add(new Paragraph("\n"));

            PdfPTable detailsTable = new PdfPTable(2) { WidthPercentage = 100 };
            detailsTable.SetWidths(new float[] { 50, 50 });

            detailsTable.AddCell(GetCell("Customer Details", headerFont, PdfPCell.ALIGN_LEFT, true));
            detailsTable.AddCell(GetCell("Order Details", headerFont, PdfPCell.ALIGN_LEFT, true));

            detailsTable.AddCell(GetCell($"Name: {invoiceModel.CustomerName}", normalFont, PdfPCell.ALIGN_LEFT));
            detailsTable.AddCell(GetCell($"Invoice Number: {invoiceModel.InvoiceNumber}", normalFont, PdfPCell.ALIGN_LEFT));

            detailsTable.AddCell(GetCell($"Mob: {invoiceModel.CustomerPhone}", normalFont, PdfPCell.ALIGN_LEFT));
            detailsTable.AddCell(GetCell($"Date: {invoiceModel.OrderDate:dd-MM-yyyy}", normalFont, PdfPCell.ALIGN_LEFT));

            detailsTable.AddCell(GetCell("", normalFont, PdfPCell.ALIGN_LEFT));
            detailsTable.AddCell(GetCell($"Sections: {string.Join(", ", invoiceModel.Sections)}", normalFont, PdfPCell.ALIGN_LEFT));

            detailsTable.AddCell(GetCell("", normalFont, PdfPCell.ALIGN_LEFT));
            detailsTable.AddCell(GetCell($"Tables: {string.Join(", ", invoiceModel.Tables)}", normalFont, PdfPCell.ALIGN_LEFT));


            document.Add(detailsTable);
            document.Add(new Paragraph("\n"));

            PdfPTable table = new PdfPTable(5) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 10, 40, 15, 15, 20 });

            table.AddCell(GetHeaderCell("Sr. No."));
            table.AddCell(GetHeaderCell("Item"));
            table.AddCell(GetHeaderCell("Quantity"));
            table.AddCell(GetHeaderCell("Unit Price"));
            table.AddCell(GetHeaderCell("Total"));

            int index = 1;
            foreach (OrderItemViewModel? item in invoiceModel.Items)
            {
                StringBuilder itemNameBuilder = new StringBuilder();
                itemNameBuilder.Append(item.ItemName);

                if (item.Modifiers != null && item.Modifiers.Any())
                {

                    foreach (ModifierViewModel? modifier in item.Modifiers)
                    {
                        itemNameBuilder.Append(Environment.NewLine + "-" + modifier.ModifierName);
                    }
                }

                StringBuilder quantityBuilder = new StringBuilder();
                quantityBuilder.Append(item.Quantity.ToString());

                if (item.Modifiers != null && item.Modifiers.Any())
                {
                    foreach (ModifierViewModel? modifier in item.Modifiers)
                    {
                        quantityBuilder.Append(Environment.NewLine + modifier.Quantity.ToString());
                    }
                }

                StringBuilder priceBuilder = new StringBuilder();
                priceBuilder.Append(item.UnitPrice.ToString("0.00"));

                if (item.Modifiers != null && item.Modifiers.Any())
                {
                    foreach (ModifierViewModel? modifier in item.Modifiers)
                    {
                        priceBuilder.Append(Environment.NewLine + modifier.Rate.ToString("0.00"));
                    }
                }

                StringBuilder totalBuilder = new StringBuilder();
                totalBuilder.Append(item.Total.ToString("0.00"));

                if (item.Modifiers != null && item.Modifiers.Any())
                {
                    foreach (ModifierViewModel? modifier in item.Modifiers)
                    {
                        decimal quantity = modifier.Quantity ?? 0;
                        decimal rate = modifier.Rate;
                        totalBuilder.Append(Environment.NewLine + (quantity * rate).ToString("0.00"));
                    }
                }

                table.AddCell(GetBorderedCell(index.ToString(), normalFont, PdfPCell.ALIGN_CENTER));
                table.AddCell(GetBorderedCell(itemNameBuilder.ToString().Trim(), normalFont, PdfPCell.ALIGN_LEFT));
                table.AddCell(GetBorderedCell(quantityBuilder.ToString().Trim(), normalFont, PdfPCell.ALIGN_CENTER));
                table.AddCell(GetBorderedCell(priceBuilder.ToString().Trim(), normalFont, PdfPCell.ALIGN_RIGHT));
                table.AddCell(GetBorderedCell(totalBuilder.ToString().Trim(), normalFont, PdfPCell.ALIGN_RIGHT));

                index++;
            }

            document.Add(table);
            document.Add(new Paragraph("\n"));

            PdfPTable summaryTable = new PdfPTable(2) { WidthPercentage = 100 };
            summaryTable.SetWidths(new float[] { 70, 30 });

            summaryTable.AddCell(GetSummaryLabelCell("Sub Total:", normalFont));
            summaryTable.AddCell(GetSummaryValueCell(invoiceModel.SubTotal.ToString("0.00"), normalFont));

            foreach (TaxBreakdownViewModel? tax in invoiceModel.TaxBreakdown)
            {
                summaryTable.AddCell(GetSummaryLabelCell($"{tax.TaxName}:", normalFont));
                summaryTable.AddCell(GetSummaryValueCell(tax.TaxValue.ToString("0.00"), normalFont));
            }

            int columnCount = summaryTable.NumberOfColumns;

            PdfPCell bottomBorderCell = new PdfPCell()
            {
                Colspan = columnCount,
                Border = Rectangle.BOTTOM_BORDER,
                BorderWidthBottom = 0.5f,
                BorderColorBottom = new BaseColor(14, 103, 167),
                FixedHeight = 10f
            };

            summaryTable.AddCell(bottomBorderCell);

            summaryTable.AddCell(GetSummaryLabelCell("Total Amount Due:", totalFont, true));
            summaryTable.AddCell(GetSummaryValueCell(invoiceModel.TotalAmountDue.ToString("0.00"), totalFont, true));

            document.Add(summaryTable);
            document.Add(new Paragraph("\n"));

            Paragraph paymentInfoMethod = new Paragraph($"Payment Method: {invoiceModel.PaymentMethod}", normalFont) { Alignment = Element.ALIGN_LEFT };
            Paragraph paymentInfoName = new Paragraph(
                "Payment Information",
                FontFactory.GetFont(FontFactory.HELVETICA, 10, Font.BOLD, new BaseColor(14, 103, 167))
            )
            {
                Alignment = Element.ALIGN_LEFT
            };

            document.Add(paymentInfoName);
            document.Add(paymentInfoMethod);
            document.Add(new Paragraph("\n"));

            Paragraph thankYou = new Paragraph("THANK YOU!", titleFont) { Alignment = Element.ALIGN_CENTER };
            document.Add(thankYou);

            document.Close();
            return File(memoryStream.ToArray(), "application/pdf", $"Invoice_{orderId}.pdf");
        }
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

    #region Helper Methods

    private static PdfPCell GetCell(string text, Font font, int alignment, bool bold = false)
    {
        PdfPCell? cell = new PdfPCell(new Phrase(text, font))
        {
            Border = Rectangle.NO_BORDER,
            Padding = 5,
            HorizontalAlignment = alignment
        };
        if (bold) cell.BackgroundColor = new BaseColor(255, 255, 255);
        if (bold) cell.Phrase.Font.Color = new BaseColor(14, 103, 167);
        return cell;
    }
    private static PdfPCell GetHeaderCell(string text)
    {
        PdfPCell? cell = new PdfPCell(new Phrase(text, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.White)))
        {
            Border = Rectangle.NO_BORDER,
            BackgroundColor = new BaseColor(0, 102, 167),
            Padding = 6,
            HorizontalAlignment = PdfPCell.ALIGN_CENTER
        };
        return cell;
    }
    private static PdfPCell GetSummaryLabelCell(string text, Font font, bool bold = false)
    {
        PdfPCell? cell = new PdfPCell(new Phrase(text, font))
        {
            Border = Rectangle.NO_BORDER,
            Padding = 5,
            HorizontalAlignment = PdfPCell.ALIGN_LEFT
        };
        if (bold) cell.BackgroundColor = new BaseColor(255, 255, 255);
        if (bold) cell.Phrase.Font.Color = new BaseColor(14, 103, 167);
        return cell;
    }
    private static PdfPCell GetSummaryValueCell(string text, Font font, bool bold = false)
    {
        PdfPCell? cell = new PdfPCell(new Phrase(text, font))
        {
            Border = Rectangle.NO_BORDER,
            Padding = 5,
            HorizontalAlignment = PdfPCell.ALIGN_RIGHT
        };
        if (bold) cell.BackgroundColor = new BaseColor(255, 255, 255);
        if (bold) cell.Phrase.Font.Color = new BaseColor(14, 103, 167);
        return cell;
    }
    private static PdfPCell GetBorderedCell(string text, Font font, int alignment)
    {
        PdfPCell? cell = new PdfPCell(new Phrase(text, font))
        {
            Border = Rectangle.BOTTOM_BORDER,
            BorderWidthBottom = 0.5f,
            BorderColorBottom = new BaseColor(179, 215, 239),
            Padding = 5,
            HorizontalAlignment = alignment
        };
        return cell;
    }

    #endregion

}
