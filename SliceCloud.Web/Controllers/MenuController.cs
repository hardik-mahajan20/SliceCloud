using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.Enums;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Attributes;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the menu module related end points.
/// </summary>
public class MenuController(ICategoryService categoryService, IItemService itemService, IModifierGroupService modifierGroupService, IUnitService unitService, IModifierService modifierService, IItemModifierGroupMapService itemModifierGroupMapService) : Controller
{
    private readonly ICategoryService _categoryService = categoryService;
    private readonly IItemService _itemService = itemService;
    private readonly IModifierGroupService _modifierGroupService = modifierGroupService;
    private readonly IUnitService _unitService = unitService;
    private readonly IModifierService _modifierService = modifierService;
    private readonly IItemModifierGroupMapService _iItemModifierGroupMapService = itemModifierGroupMapService;

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

    public async Task<IActionResult> LoadItemsByCategory(int categoryId, int pageNumber = 1, int pageSize = 5, string searchQuery = "")
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

    #region 
    public async Task<IActionResult> GetMenuData()
    {
        List<CategoryViewModel>? categories = await _categoryService.GetAllCategoriesAsync();

        List<ModifierGroupViewModel>? modifierGroups = await _modifierGroupService.GetAllModifierGroupsAsync();

        List<UnitViewModel>? units = await _unitService.GetUnitsAsync();

        List<KeyValuePair<int, string>>? itemTypes = Enum.GetValues(typeof(ItemType))
          .Cast<ItemType>()
          .Select(it => new KeyValuePair<int, string>((int)it, it.ToString()))
          .ToList();

        ItemViewModel viewModel = new()

        {
            Categories = categories,
            ModifierGroups = modifierGroups,
            Units = units,
            ItemTypes = itemTypes
        };

        return PartialView("_AddNewItemModalPartial", viewModel);
    }

    #endregion

    #region GetModifierGroupsByIds

    [HttpGet]
    public async Task<JsonResult> GetModifiersByGroup([FromQuery] List<int> modifierGroupIds)
    {

        if (modifierGroupIds == null || !modifierGroupIds.Any())
        {
            return Json(new
            {
                Error = "No modifier groups selected."
            });
        }

        List<ModifierGroup>? groups = await _modifierGroupService.GetModifierGroupsByIdsAsync(modifierGroupIds);
        List<Modifier>? modifiers = await _modifierService.GetModifiersByGroupIdsAsync(modifierGroupIds);

        var response = new
        {
            Groups = groups.Select(g => new
            {
                GroupId = g.ModifierGroupId,
                GroupName = g.ModifierGroupName
            }).ToList(),

            ModifierItems = modifiers.Select(m => new
            {
                m.ModifierId,
                m.ModifierName,
                Price = m.Rate,
                GroupId = m.ModifierGroupModifierMappings.Select(mgm => mgm.ModifierGroupId).ToList()
            }).ToList()
        };
        return Json(response);
    }

    #endregion

    #region AddMenuItem POST

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> AddMenuItem(ItemViewModel model, string ModifierGroupsJson, IFormFile? itemImage)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .Select(x => new
                {
                    Key = x.Key,
                    Errors = x.Value?.Errors.Select(e => e.ErrorMessage).ToList()
                }).ToList();

            return Json(new
            {
                success = false,
                validationErrors = errors
            });
        }


        try
        {
            bool isDuplicate = await _itemService.IsDuplicateItemAsync(model.ItemName);
            if (isDuplicate)
            {
                return Json(new
                {
                    success = false,
                    message = "Item name already exists!"
                });
            }

            List<ItemModifierGroupMapViewModel>? itemModifierGroupMapViewModels = JsonConvert.DeserializeObject<List<ItemModifierGroupMapViewModel>>(ModifierGroupsJson) ?? new List<ItemModifierGroupMapViewModel>();

            int itemId = await _itemService.AddMenuItemAsync(model, itemImage);
            if (itemId <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to add item."
                });
            }

            foreach (ItemModifierGroupMapViewModel itemModifierGroupMapViewModel in itemModifierGroupMapViewModels)
            {
                itemModifierGroupMapViewModel.ItemId = itemId;
                await _iItemModifierGroupMapService.AddItemModifierGroupMapAsync(itemModifierGroupMapViewModel);
            }

            return Json(new
            {
                success = true,
                message = "Menu item added successfully!",
                categoryId = model.CategoryId
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = "Error: " + ex.Message
            });
        }
    }

    #endregion

    #region GetItemById GET

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> GetItemById(int id)
    {
        ItemViewModel? item = await _itemService.GetItemByIdAsync(id);
        if (item == null)
        {
            return Json(new { success = false, message = "Item not found" });
        }

        List<CategoryViewModel>? categories = await _categoryService.GetAllCategoriesAsync();
        List<UnitViewModel>? units = await _unitService.GetUnitsAsync();
        List<ModifierGroup>? modifierGroups = await _modifierService.GetAllModifierGroupsAsync();

        List<ItemModifierGroupMapViewModel>? modifierGroupMappings = await _iItemModifierGroupMapService.GetMappingByItemIdAsync(id);

        List<ItemModifierGroupMapViewModel>? modifierGroupMappingViewModels = modifierGroupMappings.Select(mapping => new ItemModifierGroupMapViewModel
        {
            ModifierGroupId = mapping.ModifierGroupId,
            MinValue = mapping.MinValue,
            MaxValue = mapping.MaxValue
        }).ToList();

        EditMenuItemViewModel? editMenuItemViewModel = new()
        {
            ItemId = item.ItemId,
            ItemName = item.ItemName,
            ItemType = item.ItemType,
            CategoryId = item.CategoryId,
            Rate = item.Rate,
            Quantity = item.Quantity,
            UnitId = item.UnitId,
            IsAvailable = item.IsAvailable,
            ItemImg = item.ItemImg,
            IsDefaultTax = item.IsDefaultTax,
            TaxPercentage = item.TaxPercentage,
            ShortCode = item.ShortCode,
            Description = item.Description,
            Categories = categories,
            Units = units,
            ModifierGroups = modifierGroups,
            ModifierGroupMappings = modifierGroupMappingViewModels
        };

        return PartialView("_EditItemModalPartialView", editMenuItemViewModel);
    }

    #endregion

    #region GetModifierMappingsByItemId GET

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> GetModifierMappingsByItemId(int id)
    {
        List<ItemModifierGroupMapViewModel>? itemModifierGroupMapViewModels = await _iItemModifierGroupMapService.GetMappingByItemIdAsync(id);



        foreach (var groupMapping in itemModifierGroupMapViewModels)
        {
            ModifierGroupViewModel? modifierGroup = await _modifierGroupService.GetModifierGroupByIdAsync(groupMapping.ModifierGroupId);

            if (modifierGroup != null)
            {
                groupMapping.ModifierGroupName = modifierGroup.ModifierGroupName;

                if (groupMapping.ModifierItems == null || !groupMapping.ModifierItems.Any())
                {
                    if (modifierGroup.ModifierItems != null)
                    {
                        groupMapping.ModifierItems = modifierGroup.ModifierItems.Select(item => new ModifierItemViewModel
                        {
                            ModifierItemId = item.ModifierItemId,
                            ModifierItemName = item.ModifierItemName,
                            Price = item.Price
                        }).ToList();
                    }
                }
            }
        }

        return Json(itemModifierGroupMapViewModels);
    }

    #endregion

    #region 

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> UpdateMenuItem(EditMenuItemViewModel model, IFormFile? itemImage, string ModifierGroupsJson)
    {
        model.IsAvailable = Request.Form["Isavailable"].ToString().ToLower() == "true";
        model.IsDefaultTax = Request.Form["Isdefaulttax"].ToString().ToLower() == "true";

        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    e => e.Key,
                    e => e.Value?.Errors.Select(x => x.ErrorMessage).FirstOrDefault()
                );

            return Json(new { success = false, message = "Invalid input data!", errors = errors });
        }

        try
        {
            bool isDuplicate = await _itemService.IsDuplicateItemAsync(model.ItemName, model.ItemId);
            if (isDuplicate)
            {
                return Json(new { success = false, message = "An item with the same name already exists!" });
            }

            var modifierGroups = JsonConvert.DeserializeObject<List<ItemModifierGroupMapViewModel>>(ModifierGroupsJson) ?? new List<ItemModifierGroupMapViewModel>();


            bool isUpdated = await _itemService.UpdateMenuItemAsync(model, itemImage);
            if (!isUpdated)
            {
                return Json(new { success = false, message = "Failed to update item." });
            }

            await _iItemModifierGroupMapService.DeleteItemModifierGroupMapsByItemIdAsync(model.ItemId);

            foreach (var modifierGroup in modifierGroups)
            {
                modifierGroup.ItemId = model.ItemId;
                await _iItemModifierGroupMapService.AddItemModifierGroupMapAsync(modifierGroup);
            }

            return Json(new { success = true, message = "Menu item updated successfully!", categoryId = model.CategoryId });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error: " + ex.Message });
        }
    }

    #endregion

    #region LoadDeleteMenuItemModal

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public IActionResult LoadDeleteMenuItemModal()
    {
        return PartialView("_DeleteMenuItemModal");
    }

    #endregion

    #region DeleteMenuItem POST

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> DeleteMenuItem(int itemId)
    {
        bool isDeleted = await _itemService.DeleteItemAsync(itemId);
        if (isDeleted)
        {
            return Json(new
            {
                success = true,
                message = "Item deleted successfully!"
            });
        }
        else
        {
            return Json(new
            {
                success = false,
                message = "Error deleting item!"
            });
        }
    }

    #endregion

    #region GetAllItemIds

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> GetAllItemIds(int categoryId)
    {
        try
        {
            List<int>? itemIds = await _itemService.GetAllItemIdsAsync(categoryId);
            return Json(itemIds);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion

    #region LoadMultipleDeleteMenuItemModal

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public IActionResult LoadDeleteMultipleMenuItemModal()
    {
        return PartialView("_DeleteMultipleMenuItemModal");
    }

    #endregion

    #region DeleteMultipleItems POST

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> DeleteMultipleMenuItem([FromBody] List<int> itemIds)
    {
        if (itemIds == null || !itemIds.Any())
        {
            return Json(new { success = false, message = "No items selected." });
        }

        bool isAllItemsDeleted = await _itemService.DeleteMultipleMenuItemAsync(itemIds);
        return Json(new { success = isAllItemsDeleted });
    }

    #endregion

    #region LoadModifiers 

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> LoadModifiers()
    {
        MenuViewModel menuViewModel = new()
        {
            Modifiergroups = await _modifierGroupService.GetAllModifierGroupsAsync(),
        };
        return PartialView("_ModifierSectionPartial", menuViewModel);
    }

    #endregion

    #region GetAllModifierGroups GET

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> GetAllModifierGroups()
    {
        List<ModifierGroupViewModel>? modifierGroupViewModels = await _modifierGroupService.GetAllModifierGroupsAsync();

        if (modifierGroupViewModels == null || !modifierGroupViewModels.Any())
        {
            return Json(new { message = "No modifier groups found" });
        }

        return Json(modifierGroupViewModels);
    }

    #endregion

    #region UpdateModifierGroupOrder POST

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> UpdateModifierGroupOrder([FromBody] List<int> orderedModifierGroupIds)
    {
        try
        {
            await _modifierGroupService.UpdateModifierGroupOrderAsync(orderedModifierGroupIds);
            return Ok();
        }
        catch (UnauthorizedAccessException)
        {
            return BadRequest(new { success = false, message = "You are not authorized to perform this action." });
        }
    }

    #endregion

    #region LoadAddModifierGroupModal

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public IActionResult LoadAddModifierGroupModal()
    {
        ModifierGroupViewModel? modifierGroupViewModel = new();
        return PartialView("_AddModifierGroupModal", modifierGroupViewModel);
    }

    #endregion

    #region Add Modifier Group POST

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> AddModifierGroup(ModifierGroupViewModel modifierGroupViewModel)
    {
        if (modifierGroupViewModel == null)
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
            int newModifierGroupId = await _modifierGroupService.AddModifierGroupAsync(modifierGroupViewModel);

            return Json(new
            {
                success = true,
                message = "ModifierGroup added successfully!",
                categoryId = newModifierGroupId
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
                { "ModifierGroupName", new List<string> { ex.Message } }
            }
            });
        }
        catch (Exception)
        {
            return Json(new { success = false, message = "An unexpected error occurred while adding the category." });
        }
    }

    #endregion

    #region GetModifierGroupById

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> GetModifierGroupById(int id)
    {
        ModifierGroupViewModel? modifierGroupViewModel = await _modifierGroupService.GetModifierGroupByIdAsync(id);

        if (modifierGroupViewModel == null)
        {
            return Json(new { success = false, message = "NO Modifier Group found" });
        }
        return PartialView("_EditModifierGroupModal", modifierGroupViewModel);
    }

    #endregion

    #region Edit Modifier Group 

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> EditModifierGroup(ModifierGroupViewModel model)
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
            bool ismodifierGroupNameUpdated = await _modifierGroupService.UpdateModifierGroupAsync(model);
            if (!ismodifierGroupNameUpdated)
            {
                return Json(new { success = false, message = "Failed to update Modifier Group." });
            }

            return Json(new { success = true, message = "Modifier Group updated successfully!" });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message,
                errors = new Dictionary<string, List<string>>
            {
                { "ModifierGroupName", new List<string> { ex.Message } }
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

    #region LoadDeleteModifierGroupModal

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public IActionResult LoadDeleteModifierGroupModal()
    {
        return PartialView("_DeleteModifierGroupModal");
    }

    #endregion

    #region Delete Modifier Group 

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> DeleteModifierGroup(int modifierGroupId)
    {
        bool isModifierGroupDeleted = await _modifierGroupService.DeleteModifierGroupAsync(modifierGroupId);
        if (isModifierGroupDeleted)
        {
            return Json(new { success = true });
        }
        return Json(new { success = false });
    }

    #endregion

    #region LoadModifiersByModifierGroup

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> LoadModifiersByModifierGroup(int modifierGroupId, int pageNumber = 1, int pageSize = 5, string searchQuery = "")
    {
        PaginatedList<ModifierViewModel>? paginatedModifiers = await _modifierService.GetPaginatedModifiersByModifierGroupId(modifierGroupId, pageNumber, pageSize, searchQuery);

        ViewBag.FromRec = paginatedModifiers.FromRec;
        ViewBag.ToRec = paginatedModifiers.ToRec;
        ViewBag.TotalItems = paginatedModifiers.TotalItems;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = paginatedModifiers.TotalPages;

        return PartialView("_ModifiersPartial", paginatedModifiers);
    }

    #endregion

    #region GetAddModifier

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> GetAddModifier()
    {

        List<ModifierGroupViewModel>? modifierGroups = await _modifierGroupService.GetAllModifierGroupsAsync();

        List<UnitViewModel>? units = await _unitService.GetUnitsAsync();

        ModifierSectionViewModel viewModel = new()
        {
            ModifierGroups = modifierGroups,
            Units = units,
        };

        return PartialView("_AddNewModifierModalPartial", viewModel);
    }

    #endregion

    #region AddModifier POST

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<JsonResult> AddModifier(ModifierSectionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToList()
                );

            return Json(new
            {
                success = false,
                validationErrors = errors,
                modifierGroupIds = model.ModifierGroupIds
            });
        }

        try
        {
            int result = await _modifierService.AddModifierAsync(model);
            if (result > 0)
            {
                return Json(new { success = true });
            }

            return Json(new
            {
                success = false,
                message = "Failed to add modifier."
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = "An error occurred: " + ex.Message
            });
        }
    }

    #endregion

    #region GetModifierByIdEdit

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<ActionResult> GetModifierByIdEdit(int modifierId)
    {
        ModifierViewModel modifierViewModel = await _modifierService.GetModifierByIdAsync(modifierId);
        List<UnitViewModel>? unitViewModels = await _unitService.GetUnitsAsync();
        List<ModifierGroupViewModel>? modifierGroupViewModels = await _modifierGroupService.GetAllModifierGroupsAsync();

        ModifierSectionViewModel modifierSectionViewModel = new()
        {
            ModifierId = modifierViewModel.ModifierId,
            ModifierName = modifierViewModel.ModifierName,
            UnitId = modifierViewModel.UnitId,
            Rate = modifierViewModel.Rate,
            Quantity = modifierViewModel.Quantity,
            Description = modifierViewModel.Description,
            ModifierGroupIds = modifierViewModel.ModifierGroupIds,
            ModifierGroups = modifierGroupViewModels,
            Units = unitViewModels,
        };
        return PartialView("_EditNewModifierModalPartial", modifierSectionViewModel);
    }

    #endregion

    #region UpdateModifier

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> UpdateModifier(ModifierSectionViewModel model)
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

        int result = await _modifierService.UpdateModifierAsync(model);

        if (result > 0)
        {
            return Json(new { success = true, modifierGroupIds = model.ModifierGroupIds });
        }
        else
        {
            return Json(new { success = false, message = "Failed to update modifier." });
        }
    }

    #endregion

    #region LoadDeleteModifierModal

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public IActionResult LoadDeleteModifierModal()
    {
        return PartialView("_DeletModifierModal");
    }

    #endregion

    #region Delete Modifier POST

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> DeleteModifier(int modifierId)
    {
        bool isDeleted = await _modifierService.DeleteModifierAsync(modifierId);
        if (isDeleted)
        {
            return Json(new
            {
                success = true,
                message = "Modifier deleted successfully!"
            });
        }
        else
        {
            return Json(new
            {
                success = false,
                message = "Error deleting modifier!"
            });
        }
    }

    #endregion

    #region GetAllModifierIds

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> GetAllModifierIds(int modifierGroupId)
    {
        try
        {
            List<int>? modifierIds = await _modifierService.GetAllModifierIdsAsync(modifierGroupId);
            return Json(modifierIds);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion

    #region LoadMultipleDeleteMenuModifierModal

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public IActionResult LoadDeleteMultipleMenuModifierModal()
    {
        return PartialView("_DeleteMultiplModifierModal");
    }

    #endregion

    #region DeleteMultipleModifiers POST

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> DeleteMultipleModifier([FromBody] List<int> modifierIds)
    {
        if (modifierIds == null || !modifierIds.Any())
        {
            return Json(new { success = false, message = "No modifier selected." });
        }

        bool isAllModifierssDeleted = await _modifierService.DeleteMultipleModifierAsync(modifierIds);
        return Json(new { success = isAllModifierssDeleted });
    }

    #endregion
}
