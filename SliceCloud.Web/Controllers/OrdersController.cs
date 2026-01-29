using Microsoft.AspNetCore.Mvc;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the orders module related end points.
/// </summary>
public class OrdersController() : Controller
{

    #region Orders GET

    public IActionResult Orders()
    {
        return View();
    }

    #endregion
}
