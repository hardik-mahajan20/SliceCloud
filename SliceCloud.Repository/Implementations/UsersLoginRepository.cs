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

    #region CreateUserLogin

    public async Task<bool> CreateUserLoginAsync(UsersLogin usersLogin)
    {
        await _sliceCloudContext.UsersLogins.AddAsync(usersLogin);
        return await _sliceCloudContext.SaveChangesAsync() > 0;
    }

    #endregion

    #region UpdateUsersLogin

    public async Task<bool> UpdateUsersLoginAsync(UsersLogin usersLogin)
    {
        _sliceCloudContext.UsersLogins.Update(usersLogin);
        return await _sliceCloudContext.SaveChangesAsync() > 0;
    }

    #endregion

}
