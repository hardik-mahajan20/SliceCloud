using Microsoft.AspNetCore.Mvc;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the customer module related end points.
/// </summary>
public class CustomersController() : Controller
{

    #region Customers GET

    public IActionResult Customers()
    {
        return View();
    }

    #endregion
}
