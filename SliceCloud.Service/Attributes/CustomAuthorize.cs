using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SliceCloud.Repository.Constants;
using SliceCloud.Service.Interfaces;
using SliceCloud.Service.Utils;
using System.Security.Claims;

namespace SliceCloud.Service.Attributes
{

    [AttributeUsage(AttributeTargets.All)]
    public class CustomAuthorizeAttribute(string? requiredPermission = null, params string[] roles) : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _roles = roles;
        private readonly string _requiredPermission = requiredPermission ?? string.Empty;

        /// <summary>
        /// Handles the authorization logic asynchronously.
        /// </summary>
        /// <param name="context">The authorization filter context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (
                context.HttpContext.RequestServices.GetService(typeof(IJwtService))
                    is not IJwtService jwtService
            || context.HttpContext.RequestServices.GetService(typeof(IPermissionService))
                is not IPermissionService permissionService
            )
            {
                context.Result = new RedirectToActionResult(GeneralConstants.ERROR, GeneralConstants.HOME, null);
                return;
            }

            string? token = CookieUtils.GetJWTToken(context.HttpContext.Request);

            ClaimsPrincipal? principal = jwtService?.ValidateToken(token ?? String.Empty);
            if (principal == null)
            {
                context.Result = new RedirectToActionResult(GeneralConstants.LOGIN, GeneralConstants.AUTH, null);
                return;
            }

            context.HttpContext.User = principal;

            if (_roles.Length > 0)
            {
                string? userRole = principal.Claims.FirstOrDefault(
                    c => c.Type == ClaimTypes.Role
                )?.Value;
                if (!_roles.Contains(userRole))
                {
                    bool isAjax =
                        context.HttpContext.Request.Headers[GeneralConstants.X_REQUESTED_WITH] == GeneralConstants.XML_HTTP_REQUEST;
                    HandleAccessDenied(context, isAjax);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(_requiredPermission))
            {
                string? controllerName = context.RouteData.Values[GeneralConstants.CONTROLLER]?.ToString();
                int moduleId = GetModuleIdByControllerName(controllerName ?? string.Empty);
                if (moduleId == 0)
                {
                    context.Result = new RedirectToActionResult(GeneralConstants.ACCESS_DENIED, GeneralConstants.ERROR, null);
                    return;
                }
                // Bypass auth for QR-related controllers
                if (
                    controllerName == GeneralConstants.QR_REDIRECT
                    || controllerName == GeneralConstants.QR_CODE
                    || controllerName == GeneralConstants.QR_MENU
                )
                {
                    return;
                }
                bool hasPermission = await permissionService.RoleHasPermissionAsync(
                    principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty,
                    _requiredPermission,
                    moduleId
                );

                if (!hasPermission)
                {
                    bool isAjax =
                        context.HttpContext.Request.Headers[GeneralConstants.X_REQUESTED_WITH] == GeneralConstants.XML_HTTP_REQUEST;
                    HandleAccessDenied(context, isAjax);
                    return;
                }
            }
        }

        /// <summary>
        /// Handles unauthorized access.
        /// </summary>
        /// <param name="context">The authorization filter context.</param>
        /// <param name="isAjax">Indicates whether the request is an AJAX request.</param>
        private static void HandleUnauthorized(AuthorizationFilterContext context, bool isAjax)
        {
            if (isAjax)
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new JsonResult(new { success = false, message = GeneralConstants.UNAUTHORIZED });
            }
            else
            {
                context.Result = new RedirectToActionResult(GeneralConstants.LOGIN, GeneralConstants.AUTH, null);
            }
        }

        /// <summary>
        /// Handles access denied scenarios.
        /// </summary>
        /// <param name="context">The authorization filter context.</param>
        /// <param name="isAjax">Indicates whether the request is an AJAX request.</param>
        private static void HandleAccessDenied(AuthorizationFilterContext context, bool isAjax)
        {
            if (isAjax)
            {
                context.HttpContext.Response.StatusCode = 403;
                context.Result = new JsonResult(
                    new { success = false, message = GeneralConstants.ACCESS_DENIED }
                );
            }
            else
            {
                context.Result = new RedirectToActionResult(GeneralConstants.ACCESS_DENIED, GeneralConstants.ERROR, null);
            }
        }

        /// <summary>
        /// Retrieves the module ID based on the controller name.
        /// </summary>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The module ID if found, otherwise 0.</returns>
        private static int GetModuleIdByControllerName(string controllerName)
        {
            Dictionary<string, int> moduleMapping = new()
            {
                { SideBarOptionConstants.USERS, 1 },
                { SideBarOptionConstants.ROLE_AND_PERMISSION, 2 },
                { SideBarOptionConstants.MENU, 3 },
                { SideBarOptionConstants.TABLE_AND_SECTION, 4 },
                { SideBarOptionConstants.TAX_AND_FEES, 5 },
                { SideBarOptionConstants.ORDERS, 6 },
                { SideBarOptionConstants.CUSTOMERS, 7 },
                { SideBarOptionConstants.DASHBOARD, 8 },
                { SideBarOptionConstants.ORDER_APP, 9 },
                { SideBarOptionConstants.ORDER_APP_KOT, 10 },
                { SideBarOptionConstants.ORDER_APP_MENU, 11 },
                { SideBarOptionConstants.ORDER_APP_WAITING_LIST, 12 },
                { SideBarOptionConstants.ORDER_APP_TABLE_VIEW, 13 },
            };

            return moduleMapping.TryGetValue(controllerName, out int moduleId) ? moduleId : 0;
        }
    }
}