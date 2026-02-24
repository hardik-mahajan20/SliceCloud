using Microsoft.AspNetCore.Mvc;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.Enums;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Attributes;
using SliceCloud.Service.Implementations;
using SliceCloud.Service.Interfaces;
using SliceCloud.Service.Utils;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the tables and section module related end points.
/// </summary>
public class TableAndSectionController(ISectionService sectionService, ITableService tableService) : Controller
{

    private readonly ISectionService _sectionService = sectionService;
    private readonly ITableService _tableService = tableService;

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
                Tables = await _tableService.GetAllTablesAsync(),
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

    #region LoadTablesPaginated

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    public async Task<IActionResult> LoadTablesPaginated(int sectionId, int pageNumber, int pageSize, string searchQuery = "")
    {
        try
        {
            PaginatedList<TableViewModel>? paginatedTables = await _tableService.GetPaginatedTablesBySectionIdAsync(sectionId, pageNumber, pageSize, searchQuery);

            ViewBag.TotalItems = paginatedTables.TotalItems;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = paginatedTables.TotalPages;
            if (paginatedTables.TotalItems == 0)
            {
                ViewBag.FromRec = 0;
                ViewBag.ToRec = 0;
            }
            else
            {
                ViewBag.FromRec = paginatedTables.FromRec;
                ViewBag.ToRec = paginatedTables.ToRec;
            }
            var model = new TableViewModel
            {
                Sections = await _sectionService.GetAllSections(),
                TablesPaginated = paginatedTables
            };

            return PartialView("_TablesPartial", model);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region GetAllTableIds

    [HttpGet]
    public async Task<IActionResult> GetAllTableIds(int sectionId)
    {
        try
        {
            List<int>? itemIds = await _tableService.GetAllTableIdsAsync(sectionId);
            return Json(itemIds);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
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

    #region DeleteSection

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> DeleteSection(int sectionId)
    {
        if (sectionId <= 0)
        {
            return Json(new { success = false, message = "Invalid section ID." });
        }
        try
        {
            bool isDeleted = await _sectionService.DeleteSectionAsync(sectionId);
            if (isDeleted)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region UpdateSectionOrder

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> UpdateSectionOrder([FromBody] List<int> sortedSectionIds)
    {
        try
        {
            await _sectionService.UpdateSectionOrderAsync(sortedSectionIds);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion

    #region GetTableData

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> GetTableData(int selectedSectionId)
    {
        try
        {
            List<SectionViewModel>? sections = await _sectionService.GetAllSections();
            SectionViewModel? selectedSection = sections.FirstOrDefault(s => s.SectionId == selectedSectionId);

            TableViewModel? tableViewModel = new()
            {
                Sections = sections,
                SelectedSectionId = selectedSectionId,
                SelectedSectionName = selectedSection?.SectionName ?? string.Empty
            };

            return PartialView("_AddTableModalPartial", tableViewModel);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region AddTable

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> AddTable(TableViewModel tableViewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var allErrors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return Json(new
                {
                    success = false,
                    validationErrors = allErrors
                });
            }

            if (await _tableService.IsDuplicateTableNameAsync(tableViewModel.TableName ?? string.Empty, tableViewModel.SectionId ?? 0))
            {
                return Json(new
                {
                    success = false,
                    validationErrors = new Dictionary<string, string[]>
            {
                { "TableName", new[] { "A table with this name already exists in the selected section." } }
            }
                });
            }

            bool isTableAdded = await _tableService.AddTableAsync(tableViewModel);

            if (isTableAdded)
            {
                return Json(new
                {
                    success = true,
                    message = "Table added successfully!"
                });
            }

            return Json(new
            {
                success = false,
                message = "Failed to add table."
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region GetTableById

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> GetTableById(int tableId)
    {
        try
        {
            Repository.Models.Table? table = await _tableService.GetTableByIdAsync(tableId);
            if (table == null)
            {
                return Json(new { success = false, message = "No Table found" });
            }

            List<SectionViewModel>? sections = await _sectionService.GetAllSections();
            SectionViewModel? selectedSection = sections.FirstOrDefault(s => s.SectionId == table.SectionId);

            TableViewModel? viewModel = new()
            {
                TableId = table.TableId,
                TableName = table.TableName,
                Capacity = table.Capacity,
                Status = table.TableStatus.HasValue ? (TableStatus)table.TableStatus.Value : TableStatus.Available,
                Sections = sections,
                SelectedSectionId = table.SectionId,
                SelectedSectionName = selectedSection?.SectionName
            };

            return PartialView("_EditTableModalPartial", viewModel);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region EditTable

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> EditTable([FromForm] TableViewModel tableViewModel)
    {
        try
        {
            if (tableViewModel == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Model is null. Check AJAX request."
                });
            }

            if (!ModelState.IsValid)
            {
                var allErrors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return Json(new
                {
                    success = false,
                    validationErrors = allErrors
                });
            }
            if (await _tableService.IsDuplicateTableNameAsync(tableViewModel.TableName ?? string.Empty, tableViewModel.SectionId ?? 0, tableViewModel.TableId > 0 ? tableViewModel.TableId : null))
            {
                return Json(new
                {
                    success = false,
                    validationErrors = new Dictionary<string, string[]>
                    {
                        { "TableName", new[] { "A table with this name already exists in the selected section." } }
                    }
                });
            }

            bool isTableUpdaed = await _tableService.UpdateTableAsync(tableViewModel);

            return Json(new
            {
                success = isTableUpdaed
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region DeleteTable

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> DeleteTable([FromBody] int tableId)
    {
        try
        {
            if (tableId <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid Table ID."
                });
            }

            bool isTableDeleted = await _tableService.DeleteTableAsync(tableId);

            if (isTableDeleted)
            {
                return Json(new
                {
                    success = true,
                    message = "Table deleted successfully."
                });
            }

            return Json(new
            {
                success = false,
                message = "Failed to delete table."
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region CheckSectionTablesAvailability

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> CheckSectionTablesAvailability(int sectionId)
    {
        try
        {
            List<Repository.Models.Table>? tables = await _tableService.GetTablesBySectionIdAsync(sectionId);

            bool isAllTableAvailable = tables.All(t => t.TableStatus == (int)TableStatus.Available);

            if (isAllTableAvailable)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region DeleteMultipleTable

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> DeleteMultipleTable([FromBody] List<int> tableIds)
    {
        if (tableIds == null || !tableIds.Any())
        {
            return Json(new { success = false, message = "No items selected." });
        }

        try
        {
            bool isAllDeleted = await _tableService.DeleteMultipleTableAsync(tableIds);
            return Json(new { success = isAllDeleted });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion
}
