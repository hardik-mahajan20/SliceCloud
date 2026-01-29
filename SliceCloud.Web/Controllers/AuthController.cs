using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
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
            string? token = Request.Cookies["AuthToken"];
            if (token != null)
            {
                principal = _jwtService.ValidateToken(token);
            }

            if (principal == null)
            {
                Response.Cookies.Delete("AuthToken");
                CookieUtils.ClearCookies(HttpContext);
                SessionUtils.ClearSession(HttpContext);
                return View();
            }
            return RedirectToAction("Dashboard", "Dashboard");
        }
        catch (Exception)
        {
            TempData.SetToast("error", "An error occurred while processing your request. Please try again.");
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
                          "Password",
                          "Invalid password. Try again or reset your password."
                      );
                    }
                }
                else
                {
                    ModelState.AddModelError(
                       "Email",
                       "No user found with the provided email."
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

            HttpContext.Session.SetString("username", usersLogin.User!.UserName!);
            return RedirectToAction("Dashboard", "Dashboard");
        }
        catch
        {
            TempData.SetToast("error", "An error occurred while processing your request. Please try again.");
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
            TempData.SetToast("error", "An error occurred while processing your request. Please try again.");
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
                ModelState.AddModelError("Email", "No user found with the provided email.");
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

            TempData.SetToast("success", "A password reset link has been sent to your email.");
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception)
        {
            TempData.SetToast("error", "Failed to send reset email.");
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
                TempData.SetToast("error", "An error occurred. Please try again.");
                return RedirectToAction("Login");
            }

            bool isValid = await _authService.ValidatePasswordResetTokenAsync(token);
            if (!isValid)
            {
                TempData.SetToast("error", "Invalid or expired reset link.");
                return RedirectToAction("Login");
            }

            ResetPasswordViewModel? resetPasswordViewModel = new ResetPasswordViewModel { Token = token };
            TempData.SetToast("info", "Please reset your password");

            return View(resetPasswordViewModel);
        }
        catch (Exception)
        {
            TempData.SetToast("error", "An error occurred while processing your request. Please try again.");
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
                ModelState.AddModelError("", "Invalid or expired reset link.");
                return View(model);
            }

            bool result = await _authService.UpdateUserPasswordAsync(model.Token ?? string.Empty, model.NewPassword!);
            if (!result)
            {
                ModelState.AddModelError("", "Failed to reset password.");
                return View(model);
            }

            TempData.SetToast("success", "Your password has been successfully reset. Please log in with your new password.");
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception)
        {
            TempData.SetToast("error", "An error occurred. Please try again.");
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
            TempData.SetToast("error", "An error occurred while processing your request. Please try again.");
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
            string? oldToken = Request.Cookies["AuthToken"];
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
            TempData.SetToast("error", "An error occurred while processing your request. Please try again.");
            return RedirectToAction("Login", "Auth");
        }
    }

    #endregion
}
