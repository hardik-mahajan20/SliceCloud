using Microsoft.AspNetCore.Mvc;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Attributes;
using SliceCloud.Service.Interfaces;
using SliceCloud.Service.Utils;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the tables and section module related end points.
/// </summary>
public class TableAndSectionController(ISectionService sectionService) : Controller
{

    private readonly ISectionService _sectionService = sectionService;

    #region TableAndSection GET

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    public IActionResult TableAndSection()
    {
        try
        {
            return View();
        }
        catch
        {
            TempData.SetToast("error", "An error occurred while processing your request. Please try again.");
            return View();
        }
    }

    #endregion

    #region LoadTableSection Partial View
    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> LoadTableSection()
    {
        try
        {
            TableSectionViewModel tableSectionViewModel = new()
            {
                Sections = await _sectionService.GetAllSections()
            };

            return PartialView("_SectionPartial", tableSectionViewModel);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
    #endregion


    #region GetAddSectionModal
    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public IActionResult GetAddSectionModal()
    {
        try
        {
            return PartialView("_AddSectionModalPartial");
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
    #endregion


    #region AddSection
    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> AddSection([FromBody] SectionViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value?.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                    );

                return Json(new { success = false, validationErrors = errors });
            }

            bool isDuplicate = await _sectionService.CheckDuplicateSectionNameAsync(model.SectionName);
            if (isDuplicate)
            {
                return Json(new
                {
                    success = false,
                    validationErrors = new Dictionary<string, string[]>
            {
                { "SectionName", new[] { "A section with this name already exists." } }
            }
                });
            }

            bool isAdded = await _sectionService.AddSectionAsync(model);
            List<SectionViewModel>? updatedSections = await _sectionService.GetAllSections();

            return Json(new
            {
                success = true,
                sections = updatedSections
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
    #endregion


    #region GetSectionById
    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> GetSectionById(int id)
    {
        try
        {
            SectionViewModel? sectionViewModel = await _sectionService.GetSectionByIdAsync(id);
            if (sectionViewModel == null)
            {
                return Json(new { success = false, message = "Section not found" });
            }
            return PartialView("_EditSectionModalPartial", sectionViewModel);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
    #endregion


    #region Edit Section
    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> EditSection(SectionViewModel sectionViewModel)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToList()
            );

            return Json(new
            {
                success = false,
                message = "Validation failed. Please fix the highlighted errors.",
                errors
            });
        }

        try
        {
            bool isDuplicate = await _sectionService.CheckDuplicateSectionNameAsync(sectionViewModel.SectionName, sectionViewModel.SectionId);
            if (isDuplicate)
            {
                return Json(new
                {
                    success = false,
                    validationErrors = new Dictionary<string, string[]>
                    {
                        { "SectionName", new[] { "A section with this name already exists." } }
                    }
                });
            }

            bool isUpdated = await _sectionService.UpdateSectionAsync(sectionViewModel);
            if (!isUpdated)
            {
                return Json(new { success = false, message = "Failed to update section." });
            }

            return Json(new { success = true, message = "Section updated successfully!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
    #endregion

}
