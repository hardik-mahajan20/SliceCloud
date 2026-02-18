using ClosedXML.Excel;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.Enums;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Attributes;
using SliceCloud.Service.Interfaces;
using SliceCloud.Service.Utils;

using System.Text;
using iTextSharp.text;
using ClosedXML.Excel.Drawings;
using SliceCloud.Repository.Models;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the orders module related end points.
/// </summary>
public class OrdersController(IOrderService orderService, IWebHostEnvironment hostingEnvironment) : Controller
{

    IOrderService _orderService = orderService;
    IWebHostEnvironment _hostingEnvironment = hostingEnvironment;

    #region Orders GET

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
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

    public async Task<IActionResult> LoadOrders(string search, string status, string timeRange, DateTime? startDate, DateTime? endDate, string sortOrder = "asc", string sortColumn = "OrderDate", int page = 1, int pageSize = 10)
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

            if (string.IsNullOrEmpty(sortColumn)) sortColumn = "OrderDate";
            if (string.IsNullOrEmpty(sortOrder)) sortOrder = "asc";

            ViewData["SortColumn"] = sortColumn;
            ViewData["SortDirection"] = sortOrder;

            PaginatedList<OrderViewModel> orders = await _orderService.GetOrdersAsync(
              search, status, startDate, endDate, page, pageSize, sortColumn, sortOrder);

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
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion


    #region ExportOrders

    public Task<IActionResult> ExportOrders(string searchText, DateTime? startDate, DateTime? endDate, int? orderStatus, string sortColumn, string sortOrder)
    {
        List<Order>? orders = _orderService.GetFilteredCustomersAsync(searchText, startDate, endDate, orderStatus, sortColumn, sortOrder);
        using (XLWorkbook? workbook = new XLWorkbook())
        {
            IXLWorksheet? worksheet = workbook.Worksheets.Add("Orders");

            string? webRootPath = _hostingEnvironment.WebRootPath;
            string? imagePath = Path.Combine(webRootPath, "images/logo.png");

            if (System.IO.File.Exists(imagePath))
            {
                IXLRange? mergedRange = worksheet.Range("O2:P6").Merge();
                mergedRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                mergedRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                double mergedWidth = worksheet.Columns("O:P").Sum(c => c.Width) * 7;
                double mergedHeight = worksheet.Rows(2, 6).Sum(r => r.Height);

                IXLPicture? picture = worksheet.AddPicture(imagePath)
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

            using (MemoryStream? stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                byte[]? fileBytes = stream.ToArray();
                return Task.FromResult<IActionResult>(File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            $"Orders_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx"));
            }
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

            string logoPath = Path.Combine(_hostingEnvironment.WebRootPath, "images", "logos", "logo.png");

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
