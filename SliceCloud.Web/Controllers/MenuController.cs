using Microsoft.AspNetCore.Mvc;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Attributes;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the menu module related end points.
/// </summary>
public class MenuController(ICategoryService categoryService, IItemService itemService) : Controller
{
    private readonly ICategoryService _categoryService = categoryService;
    private readonly IItemService _itemService = itemService;

    #region Menu GET

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    public IActionResult Menu()
    {
        return View();
    }

    #endregion

    #region LoadItems

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    public async Task<IActionResult> LoadItems()
    {
        MenuViewModel model = new()
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

    #region GetCategoryById

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        CategoryViewModel? categoryViewModel = await _categoryService.GetCategoryByIdAsync(id);

        if (categoryViewModel == null)
        {
            return Json(new { success = false, message = "NO category found" });
        }
        return PartialView("_EditCategoryModal", categoryViewModel);
    }

    #endregion

    #region Edit Category 

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> EditCategory(CategoryViewModel model)
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
            bool isCategoryUpdated = await _categoryService.UpdateAsync(model);
            if (!isCategoryUpdated)
            {
                return Json(new { success = false, message = "Failed to update category." });
            }

            return Json(new { success = true, message = "Category updated successfully!" });
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

        catch (KeyNotFoundException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
        catch (Exception)
        {
            return Json(new { success = false, message = "An unexpected error occurred while updating the category." });
        }
    }

    #endregion

    #region LoadDeleteCategoryModal

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public IActionResult LoadDeleteCategoryModal()
    {
        return PartialView("_DeleteCategoryModal");
    }

    #endregion

    #region Delete Category 

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int categoryId)
    {
        bool isCategoryDeleted = await _categoryService.DeleteCategoryAsync(categoryId);
        if (isCategoryDeleted)
        {
            return Json(new { success = true });
        }
        return Json(new { success = false });
    }
    #endregion

    #region LoadItemsByCategory

    public async Task<IActionResult> LoadItemsByCategory(int categoryId, int pageNumber, int pageSize, string searchQuery = "")
    {
        PaginatedList<ItemViewModel>? paginatedItems = await _itemService.GetPaginatedItemsByGroupIdAsync(categoryId, pageNumber, pageSize, searchQuery);

        ViewBag.FromRec = paginatedItems.FromRec;
        ViewBag.ToRec = paginatedItems.ToRec;
        ViewBag.TotalItems = paginatedItems.TotalItems;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = paginatedItems.TotalPages;

        return PartialView("_ItemsPartial", paginatedItems);
    }

    #endregion

    #region GetAllCategories
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        List<CategoryViewModel>? categoryViewModels = await _categoryService.GetAllCategoriesAsync();
        return Json(categoryViewModels);
    }
    #endregion
}
