using Microsoft.AspNetCore.Mvc;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the menu module related end points.
/// </summary>
public class MenuController() : Controller
{

    #region Menu GET

    public IActionResult Menu()
    {
        return View();
    }

    #endregion
}
