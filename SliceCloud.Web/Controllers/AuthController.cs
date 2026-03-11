using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;
using SliceCloud.Service.Utils;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the authentication related end points.
/// </summary>
public class AuthController(IAuthService authService, IJwtService jwtService, IEmailSenderService emailSenderService) : Controller
{
    private readonly IAuthService _authService = authService;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IEmailSenderService _emailSenderService = emailSenderService;

    #region Login GET

    [HttpGet]
    public IActionResult Login()
    {
        try
        {
            (string? Email, string? Username)? user = SessionUtils.GetUser(HttpContext);

            if (user == null)
                return View();


            ClaimsPrincipal? principal = null;
            string? token = Request.Cookies[GeneralConstants.AUTH_TOKEN];
            if (token != null)
            {
                principal = _jwtService.ValidateToken(token);
            }

            if (principal == null)
            {
                Response.Cookies.Delete(GeneralConstants.AUTH_TOKEN);
                CookieUtils.ClearCookies(HttpContext);
                SessionUtils.ClearSession(HttpContext);
                return View();
            }
            return RedirectToAction(SideBarOptionConstants.DASHBOARD, SideBarOptionConstants.DASHBOARD);
        }
        catch (Exception)
        {
            TempData.SetToast(GeneralConstants.ERROR, ErrorConstants.ERROR_ON_REQUEST_PROCESSING);
            return View();
        }
    }

    #endregion

    #region Login POST

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            UsersLogin? usersLogin = await _authService.AuthenticateUserAsync(
                         loginViewModel.Email!.ToLower(),
                         loginViewModel.Password!
                     );

            if (usersLogin is null)
            {
                UsersLogin? userExists = await _authService.GetUserLoginByEmailAsync(loginViewModel.Email.ToLower());
                if (userExists is not null)
                {
                    if (userExists.IsFirstLogin)
                    {
                        string resetToken = await _authService.GeneratePasswordResetTokenAsync(
                        userExists.Email!
                    );

                        return RedirectToAction(
                            "ResetPassword",
                            "Auth",
                            new { token = resetToken }
                        );
                    }
                    else
                    {
                        ModelState.AddModelError(
                          UserConstants.PASSWORD,
                          ErrorConstants.INVALID_PASSWORD
                      );
                    }
                }
                else
                {
                    ModelState.AddModelError(
                       UserConstants.EMAIL,
                       ErrorConstants.NO_USER_FOUND_WITH_PROVIDED_EMAIL
                   );
                }
                return View(loginViewModel);
            }

            string token = await _jwtService.GenerateJwtTokenAsync(loginViewModel.Email, loginViewModel.RememberMe);
            CookieUtils.SaveJWTToken(Response, token);

            if (loginViewModel.RememberMe)
            {
                CookieUtils.SaveUserData(Response, usersLogin);
            }

            HttpContext.Session.SetString(UserConstants.USER_NAME, usersLogin.User!.UserName!);
            return RedirectToAction("Dashboard", "Dashboard");
        }
        catch
        {
            TempData.SetToast(GeneralConstants.ERROR, ErrorConstants.ERROR_ON_REQUEST_PROCESSING);
            return View("Error");
        }
    }

    #endregion

    #region ForgotPassword GET

    [HttpGet]
    public IActionResult ForgotPassword(string email = "")
    {
        try
        {
            return View(new ForgotPasswordViewModel { Email = email });
        }
        catch (Exception)
        {
            TempData.SetToast(GeneralConstants.ERROR, ErrorConstants.ERROR_ON_REQUEST_PROCESSING);
            return View("Error");
        }
    }

    #endregion

    #region ForgotPassword POST

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            UsersLogin? userExists = await _authService.GetUserLoginByEmailAsync(model.Email);
            if (userExists is null)
            {
                ModelState.AddModelError(UserConstants.EMAIL, ErrorConstants.NO_USER_FOUND_WITH_PROVIDED_EMAIL);
                return View(model);
            }

            string resetToken = await _authService.GeneratePasswordResetTokenAsync(model.Email);
            string? resetLink = Url.Action(
                "ResetPassword",
                "Auth",
                new { token = resetToken },
                Request.Scheme
            );

            await _emailSenderService.SendResetPasswordEmailAsync(model.Email, resetLink);

            TempData.SetToast(GeneralConstants.SUCCESS, SuccessConstants.PASSWORD_RESET_LINK_SENT);
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception)
        {
            TempData.SetToast(GeneralConstants.ERROR, ErrorConstants.FAILED_TO_SEND_RESET_EMAIL);
            return View(model);
        }
    }

    #endregion

    #region ResetPassword GET

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData.SetToast(GeneralConstants.ERROR, ErrorConstants.ERROR_ON_REQUEST_PROCESSING);
                return RedirectToAction("Login");
            }

            bool isValid = await _authService.ValidatePasswordResetTokenAsync(token);
            if (!isValid)
            {
                TempData.SetToast(GeneralConstants.ERROR, ErrorConstants.INVALID_EXPIRED_LINK);
                return RedirectToAction("Login");
            }

            ResetPasswordViewModel? resetPasswordViewModel = new ResetPasswordViewModel { Token = token };
            TempData.SetToast("info", "Please reset your password");

            return View(resetPasswordViewModel);
        }
        catch (Exception)
        {
            TempData.SetToast(GeneralConstants.ERROR, ErrorConstants.ERROR_ON_REQUEST_PROCESSING);
            return RedirectToAction("Login");
        }
    }

    #endregion

    #region ResetPassword POST

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isValid = await _authService.ValidatePasswordResetTokenAsync(model.Token ?? string.Empty);
            if (!isValid)
            {
                ModelState.AddModelError(string.Empty, ErrorConstants.INVALID_EXPIRED_LINK);
                return View(model);
            }

            bool result = await _authService.UpdateUserPasswordAsync(model.Token ?? string.Empty, model.NewPassword!);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, ErrorConstants.FAILED_TO_SEND_RESET_PASSWORD);
                return View(model);
            }

            TempData.SetToast(GeneralConstants.SUCCESS, SuccessConstants.PASSWORD_RESET_LINK_SENT_LOGIN_AGAIN);
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception)
        {
            TempData.SetToast(GeneralConstants.ERROR, ErrorConstants.ERROR_ON_REQUEST_PROCESSING);
            return View(model);
        }
    }

    #endregion

    #region LogOut Method

    public IActionResult Logout()
    {
        try
        {
            CookieUtils.ClearCookies(HttpContext);
            SessionUtils.ClearSession(HttpContext);
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception)
        {
            TempData.SetToast(GeneralConstants.ERROR, ErrorConstants.ERROR_ON_REQUEST_PROCESSING);
            return RedirectToAction("Login", "Auth");
        }
    }

    #endregion

    #region RefreshToken 

    [HttpPost]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            string? oldToken = Request.Cookies[GeneralConstants.AUTH_TOKEN];
            if (string.IsNullOrEmpty(oldToken))
            {
                return RedirectToAction("Login", "Auth");
            }

            ClaimsPrincipal? principal = _jwtService.ValidateToken(oldToken);

            if (principal == null)
                return RedirectToAction("Login", "Auth");

            string? email = principal.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Auth");

            string? newToken = await _jwtService.GenerateJwtTokenAsync(email);
            CookieUtils.SaveJWTToken(Response, newToken);

            return Ok(new { success = true });
        }
        catch (Exception)
        {
            TempData.SetToast(GeneralConstants.ERROR, ErrorConstants.ERROR_ON_REQUEST_PROCESSING);
            return RedirectToAction("Login", "Auth");
        }
    }

    #endregion
}
