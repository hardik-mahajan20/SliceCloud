using Microsoft.AspNetCore.Mvc;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Attributes;
using SliceCloud.Service.Interfaces;
using SliceCloud.Service.Utils;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the role and permissions module related end points.
/// </summary>
public class RoleAndPermissionController(IRolesService rolesService, IPermissionService permissionService) : Controller
{
    private readonly IRolesService _rolesService = rolesService;
    private readonly IPermissionService _permissionService = permissionService;

    #region RoleAndPermission GET

    public async Task<IActionResult> RoleAndPermission()
    {
        try
        {
            List<Role> roles = await _rolesService.GetAllRolesAsync();

            List<RoleViewModel> roleViewModels = roles.Select(r => new RoleViewModel
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName
            }).ToList();

            return View(roleViewModels);
        }
        catch
        {
            TempData.SetToast("error", "An error occurred while processing your request. Please try again.");
            return View();
        }
    }

    #endregion

    #region Permission GET

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> Permission(int id)
    {
        try
        {
            string? userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            bool HasRolesPermissionAddEdit = false;
            bool HasRolesPermissionDelete = false;

            if (!string.IsNullOrWhiteSpace(userRole))
            {
                HasRolesPermissionAddEdit =
                    await _permissionService.RoleHasPermissionAsync(userRole, "CanAddEdit", 2);

                HasRolesPermissionDelete =
                    await _permissionService.RoleHasPermissionAsync(userRole, "CanDelete", 2);
            }


            ViewBag.UserRole = userRole;
            ViewBag.CanAddEdit = HasRolesPermissionAddEdit;
            ViewBag.CanDelete = HasRolesPermissionDelete;

            RoleAndPermissionsViewModel roleAndPermissionsViewModel = await _permissionService.GetAllPermissionsAsync(id);
            return View(roleAndPermissionsViewModel);
        }
        catch
        {
            TempData.SetToast("error", "An error occurred while processing your request. Please try again.");
            return View();
        }
    }

    #endregion

    #region UpdatePermission

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public IActionResult UpdatePermission(RoleAndPermissionsViewModel roleAndPermissionsViewModel)
    {
        try
        {
            int id = roleAndPermissionsViewModel.RoleId;
            if (_permissionService.UpdateAllPermissionsAsync(roleAndPermissionsViewModel).Result)
            {
                TempData.SetToast("success", "Permissions Updated Successfully!");
                return RedirectToAction("Permission", new { id });
            }
            else
            {
                TempData.SetToast("error", "Permissions Updated Failed!");
            }
            return RedirectToAction("Permission", new { id = id });
        }
        catch
        {
            TempData.SetToast("error", "An error occurred while processing your request. Please try again.");
            return View();
        }
    }

    #endregion
}
