using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.Models;

namespace SliceCloud.Service.Utils;

public static class SessionUtils
{
    /// <summary>
    /// Stores user data (email and username) in the session.
    /// </summary>
    /// <param name="httpContext">The HTTP context to store the session data in.</param>
    /// <param name="user">The user object containing email and username.</param>
    public static void SetUser(HttpContext httpContext, User user)
    {
        if (user != null)
        {
            string userData = JsonSerializer.Serialize(user);
            httpContext.Session.SetString(GenralConstants.USER_DATA, userData);
        }
    }

    /// <summary>
    /// Retrieves user data (email and username) from the session or cookies.
    /// </summary>
    /// <param name="httpContext">The HTTP context to retrieve the session or cookie data from.</param>
    /// <returns>A tuple containing the user's email and username, or null if not found.</returns>
    public static (string? Email, string? Username)? GetUser(HttpContext httpContext)
    {
        string? userData = httpContext.Session.GetString(GenralConstants.USER_DATA);

        if (string.IsNullOrEmpty(userData))
        {
            httpContext.Request.Cookies.TryGetValue(GenralConstants.USER_DATA, out userData);
        }

        if (string.IsNullOrEmpty(userData))
            return null;

        try
        {
            User? user = JsonSerializer.Deserialize<User>(userData);
            return user is not null ? (user.Email, user.UserName) : null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Clears all session data.
    /// </summary>
    /// <param name="httpContext">The HTTP context to clear the session data from.</param>
    public static void ClearSession(HttpContext httpContext) => httpContext.Session.Clear();

}
