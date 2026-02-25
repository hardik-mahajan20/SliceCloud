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


}
