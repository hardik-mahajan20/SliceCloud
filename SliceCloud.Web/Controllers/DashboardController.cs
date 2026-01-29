using Microsoft.AspNetCore.Mvc;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the dashboard module related end points.
/// </summary>
public class DashboardController() : Controller
{

    #region Dashboard GET

    public IActionResult Dashboard()
    {
        return View();
    }

    #endregion
}
