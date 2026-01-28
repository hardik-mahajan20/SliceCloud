using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
                context.Result = new RedirectToActionResult("Error", "Home", null);
                return;
            }

            string? token = CookieUtils.GetJWTToken(context.HttpContext.Request);

            ClaimsPrincipal? principal = jwtService?.ValidateToken(token ?? "");
            if (principal == null)
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
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
                        context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
                    HandleAccessDenied(context, isAjax);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(_requiredPermission))
            {
                string? controllerName = context.RouteData.Values["controller"]?.ToString();
                int moduleId = GetModuleIdByControllerName(controllerName ?? string.Empty);
                if (moduleId == 0)
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
                    return;
                }
                // Bypass auth for QR-related controllers
                if (
                    controllerName == "QRRedirect"
                    || controllerName == "QRCode"
                    || controllerName == "QRMenu"
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
                        context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
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
                context.Result = new JsonResult(new { success = false, message = "Unauthorized." });
            }
            else
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
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
                    new { success = false, message = "Access Denied." }
                );
            }
            else
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
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
                { "Users", 1 },
                { "RoleAndPermission", 2 },
                { "Menu", 3 },
                { "TableAndSection", 4 },
                { "TaxesAndFees", 5 },
                { "Orders", 6 },
                { "Customers", 7 },
                { "Dashboard", 8 },
                { "OrderApp", 9 },
                { "OrderAppKOT", 10 },
                { "OrderAppMenu", 11 },
                { "OrderAppWaitingList", 12 },
                { "OrderAppTableView", 13 },
            };

            return moduleMapping.TryGetValue(controllerName, out int moduleId) ? moduleId : 0;
        }
    }
}