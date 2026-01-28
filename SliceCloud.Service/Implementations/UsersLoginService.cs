using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class UsersLoginService(IUsersLoginRepository usersLoginRepository) : IUsersLoginService
{
    private readonly IUsersLoginRepository _usersLoginRepository = usersLoginRepository;

    #region CreateUserLogin

    public async Task CreateUserLoginAsync(UsersLoginViewModel usersLoginViewModel)
    {
        try
        {
            UsersLogin usersLogin = new()
            {
                PasswordHash = usersLoginViewModel.HashPassword,
                Email = usersLoginViewModel.Email,
                UserId = usersLoginViewModel.UserId,
                RoleId = usersLoginViewModel.RoleId,
                IsFirstLogin = true
            };
            await _usersLoginRepository.CreateUserLoginAsync(usersLogin);
        }
        catch (Exception ex)
        {
            string error = ex.InnerException?.Message ?? ex.Message;
            throw new Exception("An error occurred while creating the user login. Details: " + error, ex);
        }
    }

    #endregion
}
