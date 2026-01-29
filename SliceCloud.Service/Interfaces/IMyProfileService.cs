using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Service.Interfaces;

public interface IMyProfileService
{
    /// <summary>
    /// Retrieves the profile details of a user asynchronously.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve the profile for.</param>
    /// <returns>A task that returns the user's profile view model.</returns>
    Task<MyProfileViewModel?> GetProfileByIdAsync(int userId);

    /// <summary>
    /// Checks if the user name exists or not.
    /// </summary>
    /// <param name="username">The username of the user to check for.</param>
    /// <param name="currentUserId">The id of the user to check for</param>
    /// <returns>A task that returns the true if the username found else false.</returns>
    Task<bool> IsUsernameTakenAsync(string username, int currentUserId);

    /// <summary>
    /// Updates user profile data asynchronously.
    /// </summary>
    /// <param name="userId">The id of the user to update.</param>
    /// <param name="updateProfileViewModel">The updateProfileViewModel which is update data of the user.</param>
    /// <returns>A task that returns the updateProfileViewModel model.</returns>
    Task<MyProfileViewModel?> UpdateProfileAsync(int userId, MyProfileViewModel updateProfileViewModel);
}
