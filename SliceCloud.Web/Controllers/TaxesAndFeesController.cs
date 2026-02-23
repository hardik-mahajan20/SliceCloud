using Microsoft.AspNetCore.Mvc;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Attributes;
using SliceCloud.Service.Interfaces;
using SliceCloud.Service.Utils;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the tax and fees module related end points.
/// </summary>
public class TaxesAndFeesController(ITaxesFeesService taxesFeesService) : Controller
{
    ITaxesFeesService _taxesFeesService = taxesFeesService;

    #region TaxesAndFees Index

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public IActionResult TaxesAndFees()
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

    #region GetTaxesAndFeesTable

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> GetTaxesAndFeesTable(string search = "", int page = 1, int pageSize = 5, string sortColumn = "TaxName", string sortDirection = "asc")
    {
        try
        {
            PaginatedList<TaxesFeesViewModel>? taxesFeesViewModels = await _taxesFeesService.GetTaxesAndFeesAsync(search, page, pageSize, sortColumn, sortDirection);

            ViewBag.PageNumber = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = taxesFeesViewModels.TotalItems;
            ViewBag.TotalPages = taxesFeesViewModels.TotalPages;
            if (taxesFeesViewModels.TotalItems == 0)
            {
                ViewBag.FromRec = 0;
                ViewBag.ToRec = 0;
            }
            else
            {
                ViewBag.FromRec = ((page - 1) * pageSize) + 1;
                ViewBag.ToRec = Math.Min(page * pageSize, taxesFeesViewModels.TotalItems);
            }

            return PartialView("_TaxFeesPartialView", taxesFeesViewModels);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region AddTax

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> AddTax(TaxesFeesViewModel taxesFeesViewModel)
    {
        try
        {
            if (await _taxesFeesService.IsDuplicateTaxNameAsync(taxesFeesViewModel.TaxName ?? string.Empty, taxesFeesViewModel.TaxId))
            {
                ModelState.AddModelError("TaxName", "Tax name already exists.");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return Json(new
                {
                    success = false,
                    errors
                });
            }

            bool isTaxAdded = await _taxesFeesService.AddTaxAsync(taxesFeesViewModel);
            return Json(new
            {
                success = isTaxAdded,
                message = isTaxAdded ? "Tax added successfully!" : "Failed to add tax."
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion


    #region GetTaxById

    public async Task<JsonResult> GetTaxById(int id)
    {
        try
        {
            var tax = await _taxesFeesService.GetTaxByIdAsync(id);
            if (tax == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Tax not found."
                });
            }

            return Json(new
            {
                success = true,
                taxId = tax.TaxId,
                taxName = tax.TaxName,
                taxType = tax.TaxType,
                taxValue = tax.TaxValue,
                isEnabled = tax.IsEnabled,
                isDefault = tax.IsDefault
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region UpdateTax

    [HttpPost]
    public async Task<IActionResult> UpdateTax(TaxesFeesViewModel model)
    {
        try
        {
            if (model.TaxName == "Other")
            {
                ModelState.AddModelError("TaxName", "Tax name 'Other' cannot be used.");
            }
            if (await _taxesFeesService.IsDuplicateTaxNameAsync(model.TaxName ?? string.Empty, model.TaxId))
            {
                ModelState.AddModelError("TaxName", "Tax name already exists.");
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new { success = false, errors });
            }

            bool isTaxUpdate = await _taxesFeesService.UpdateTaxAsync(model);
            if (!isTaxUpdate)
            {
                return StatusCode(500, "Error updating tax.");
            }

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region DeleteTax 

    [HttpPost]
    public async Task<IActionResult> DeleteTax(int taxId)
    {
        try
        {
            if (taxId == 0)
            {
                return Json(new { success = false, message = "Please provide a valid id" });
            }
            bool deleted = await _taxesFeesService.DeleteTaxAsync(taxId);
            return Json(new { success = deleted });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region ToggleTaxField

    [CustomAuthorize("CanAddEdit", "Manager", "Admin", "Chef")]
    [HttpPost]
    public async Task<IActionResult> ToggleTaxField(int taxId, bool isChecked, string field)
    {
        try
        {
            await _taxesFeesService.ToggleTaxFieldAsync(taxId, isChecked, field);
            return Json(new { success = true, message = $"{field} updated." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region GetEnabledTaxes

    public async Task<IActionResult> GetEnabledTaxes()
    {
        try
        {
            List<TaxViewModel>? taxViewModels = await _taxesFeesService.GetEnabledTaxesAsync();
            return Json(taxViewModels);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region GetItemSpecificTaxes

    public async Task<IActionResult> GetItemSpecificTaxes([FromBody] List<int> itemIds)
    {
        try
        {
            if (itemIds == null || !itemIds.Any())
                return BadRequest("Item ID list is empty.");

            List<ItemSpecificTaxViewModel>? itemSpecificTaxViewModels = await _taxesFeesService.GetDefaultItemTaxesAsync(itemIds);
            return Json(itemSpecificTaxViewModels);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion

}
