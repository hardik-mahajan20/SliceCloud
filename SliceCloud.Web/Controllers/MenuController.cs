using Microsoft.AspNetCore.Mvc;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the menu module related end points.
/// </summary>
public class MenuController(ICategoryService categoryService) : Controller
{
    private readonly ICategoryService _categoryService = categoryService;

    #region Menu GET

    public IActionResult Menu()
    {
        return View();
    }

    #endregion

    #region LoadItems

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
}
