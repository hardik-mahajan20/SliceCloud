using Microsoft.AspNetCore.Mvc;
using SliceCloud.Repository.Constants;
using SliceCloud.Service.Attributes;
using SliceCloud.Service.Utils;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the dashboard module related end points.
/// </summary>
public class DashboardController() : Controller
{

    #region Dashboard GET

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    public IActionResult Dashboard()
    {
        try
        {
            return View();
        }
        catch (Exception)
        {
            TempData.SetToast("error", "An error occurred while processing your request. Please try again.");
            return View();
        }
    }

    #endregion

}
