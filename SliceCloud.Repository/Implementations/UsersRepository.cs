using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class UsersRepository(SliceCloudContext sliceCloudContext) : IUsersRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllUsersAsQuearyable

    public IQueryable<User> GetAllUsersAsQuearyable()
    {
        return _sliceCloudContext.Users.AsQueryable();
    }

    #endregion

    #region GetUserById

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _sliceCloudContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    #endregion

    #region CreateUser

    public async Task<bool> CreateUserAsync(User user)
    {
        _sliceCloudContext.Users.Add(user);
        return await _sliceCloudContext.SaveChangesAsync() > 0;
    }

    #endregion

    #region UpdateUserAsync

    public async Task<bool> UpdateUserAsync(User user)
    {
        _sliceCloudContext.Users.Update(user);
        return await _sliceCloudContext.SaveChangesAsync() > 0;
    }

    #endregion

}
