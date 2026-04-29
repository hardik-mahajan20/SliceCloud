using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.Models;

namespace SliceCloud.Service.Utils;

public static class CookieUtils
{
    /// <summary>
    /// Saves a JWT token in the response cookies.
    /// </summary>
    /// <param name="response">The HTTP response to append the cookie to.</param>
    /// <param name="token">The JWT token to save.</param>
    public static void SaveJWTToken(HttpResponse response, string token)
    {
        response.Cookies.Append(GeneralConstants.AUTH_TOKEN, token, new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            Expires = DateTime.UtcNow.AddMinutes(15)
        });
    }

    /// <summary>
    /// Retrieves the JWT token from the request cookies.
    /// </summary>
    /// <param name="request">The HTTP request to retrieve the cookie from.</param>
    /// <returns>The JWT token if found, otherwise null.</returns>
    public static string? GetJWTToken(HttpRequest request)
    {
        _ = request.Cookies.TryGetValue(GeneralConstants.AUTH_TOKEN, out string? token);
        return token;
    }

    /// <summary>
    /// Saves user data (email and username) in the response cookies.
    /// </summary>
    /// <param name="response">The HTTP response to append the cookie to.</param>
    /// <param name="user">The user object containing email and username.</param>
    public static void SaveUserData(HttpResponse response, UsersLogin user)
    {
        string userData = JsonSerializer.Serialize(new { user.Email, user.User!.UserName });

        CookieOptions? cookieOptions = new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(3),
            HttpOnly = true,
            Secure = true,
            IsEssential = true
        };
        response.Cookies.Append(GeneralConstants.USER_DATA, userData, cookieOptions);
    }

    /// <summary>
    /// Clears all cookies related to authentication and user data.
    /// </summary>
    /// <param name="httpContext">The HTTP context to delete the cookies from.</param>
    public static void ClearCookies(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete(GeneralConstants.AUTH_TOKEN);
        httpContext.Response.Cookies.Delete(GeneralConstants.USER_DATA);
    }
}