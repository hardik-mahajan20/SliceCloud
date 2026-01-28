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
}
