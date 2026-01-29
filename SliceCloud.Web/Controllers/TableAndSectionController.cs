using Microsoft.AspNetCore.Mvc;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the tables and section module related end points.
/// </summary>
public class TableAndSectionController() : Controller
{

    #region TableAndSection GET

    public IActionResult TableAndSection()
    {
        return View();
    }

    #endregion
}
