using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Service.Interfaces;

public interface IUsersLoginService
{
    /// <summary>
    /// Creates a new user's login details asynchronously.
    /// </summary>
    /// <param name="usersLoginViewModel">The view model containing user's login details.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateUserLoginAsync(UsersLoginViewModel usersLoginViewModel);
}
