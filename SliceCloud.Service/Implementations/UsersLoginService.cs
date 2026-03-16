using SliceCloud.Repository.Constants;
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
            await _usersLoginRepository.AddUserLoginAsync(usersLogin); ;
        }
        catch (Exception ex)
        {
            string error = ex.InnerException?.Message ?? ex.Message;
            throw new Exception(ErrorConstants.ERROR_OCCURRED_WHILE_CREATING_USER + error, ex);
        }
    }

    #endregion
}
