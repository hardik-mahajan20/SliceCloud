using Microsoft.AspNetCore.Mvc;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the tax and fees module related end points.
/// </summary>
public class TaxesAndFeesController() : Controller
{

    #region TaxesAndFees GET

    public IActionResult TaxesAndFees()
    {
        return View();
    }

    #endregion
}
