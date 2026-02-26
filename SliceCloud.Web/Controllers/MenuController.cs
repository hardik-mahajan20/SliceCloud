using Microsoft.AspNetCore.Mvc;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Attributes;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the menu module related end points.
/// </summary>
public class MenuController(ICategoryService categoryService) : Controller
{
    private readonly ICategoryService _categoryService = categoryService;

    #region Menu GET

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    public IActionResult Menu()
    {
        return View();
    }

    #endregion

    #region LoadItems

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    public async Task<IActionResult> LoadItems(int pageNumber, int pageSize)
    {
        var model = new MenuViewModel
        {
            Categories = await _categoryService.GetAllCategoriesAsync(),
        };

        return PartialView("_ItemSectionPartial", model);
    }

    #endregion

    #region UpdateCategoryOrder POST

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> UpdateCategoryOrder([FromBody] List<int> orderedCategoryIds)
    {
        try
        {
            await _categoryService.UpdateCategoryOrderAsync(orderedCategoryIds);
            return Ok();
        }
        catch (UnauthorizedAccessException)
        {
            return BadRequest(new { success = false, message = "You are not authorized to perform this action." });
        }
    }

    #endregion

    #region LoadAddCategoryModal

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public IActionResult LoadAddCategoryModal()
    {
        CategoryViewModel? categoryViewModel = new();
        return PartialView("_AddCategoryModal", categoryViewModel);
    }

    #endregion

    #region Add Category POST

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> AddCategory(CategoryViewModel categoryViewModel)
    {
        if (categoryViewModel == null)
        {
            return Json(new { success = false, message = "Invalid request: No data received." });
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToList()
            );

            return Json(new { success = false, errors });
        }

        try
        {
            int newCategoryId = await _categoryService.AddCategoryAsync(categoryViewModel);

            return Json(new
            {
                success = true,
                message = "Category added successfully!",
                categoryId = newCategoryId
            });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message,
                errors = new Dictionary<string, List<string>>
            {
                { "CategoryName", new List<string> { ex.Message } }
            }
            });
        }
        catch (Exception)
        {
            return Json(new { success = false, message = "An unexpected error occurred while adding the category." });
        }
    }

    #endregion
}
