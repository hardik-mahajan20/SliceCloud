using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class UsersRepository(SliceCloudContext sliceCloudContext) : IUsersRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllUsersAsQueryable

    public IQueryable<User> GetAllUsersAsQueryable()
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

    #region AddUser

    public async Task<int> AddUserAsync(User user)
    {
        await _sliceCloudContext.Users.AddAsync(user);
        await _sliceCloudContext.SaveChangesAsync();
        return user.UserId;
    }

    #endregion

    #region UpdateUser

    public async Task<int> UpdateUserAsync(User user)
    {
        _sliceCloudContext.Users.Update(user);
        await _sliceCloudContext.SaveChangesAsync();
        return user.UserId;
    }

    #endregion

}
