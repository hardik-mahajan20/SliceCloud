using System.Text;

namespace SliceCloud.Service.Utils;

public static class PasswordUtils
{
    #region HashPassword

    /// <summary>
    /// Converts the password to the hashed password.
    /// </summary>
    /// <param name="password">The password which needs to be converted</param>
    /// <returns>The hashed password.</returns>
    public static string HashPassword(string password)
    {
        return Convert.ToBase64String(
            System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(password))
        );
    }

    #endregion
}
