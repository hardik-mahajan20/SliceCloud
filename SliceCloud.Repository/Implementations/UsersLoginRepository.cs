using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class UsersLoginRepository(SliceCloudContext sliceCloudContext) : IUsersLoginRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetUsersLoginAsQueryable

    public IQueryable<UsersLogin> GetUsersLoginAsQueryable()
    {
        return _sliceCloudContext.UsersLogins.AsQueryable();
    }

    #endregion

    #region GetUsersLoginWithUserAsQueryable

    public IQueryable<UsersLogin> GetUsersLoginWithUserAsQueryable()
    {
        return _sliceCloudContext.UsersLogins.Include(u => u.User).AsQueryable();
    }

    #endregion

    #region AddUserLogin

    public async Task<int> AddUserLoginAsync(UsersLogin usersLogin)
    {
        await _sliceCloudContext.UsersLogins.AddAsync(usersLogin);
        await _sliceCloudContext.SaveChangesAsync();
        return usersLogin.UserLoginId;
    }

    #endregion

    #region UpdateUsersLogin

    public async Task<int> UpdateUsersLoginAsync(UsersLogin usersLogin)
    {
        _sliceCloudContext.UsersLogins.Update(usersLogin);
        await _sliceCloudContext.SaveChangesAsync();
        return usersLogin.UserLoginId;
    }

    #endregion

}
