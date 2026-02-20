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

    #region IsEmailExists

    public async Task<bool> IsEmailExistsAsync(string email, int? userId)
    {
        if (userId is not null)
        {
            return await _sliceCloudContext.Users.AnyAsync(u => u.Email == email && u.UserId != userId);
        }
        return await _sliceCloudContext.Users.AnyAsync(u => u.Email == email);
    }

    #endregion

    #region IsPhoneExists

    public async Task<bool> IsPhoneExistsAsync(string phone, int? userId)
    {
        if (userId is not null)
        {
            return await _sliceCloudContext.Users.AnyAsync(u => u.PhoneNumber == phone && u.UserId != userId);
        }
        return await _sliceCloudContext.Users.AnyAsync(u => u.PhoneNumber == phone);
    }
    #endregion

    #region IsUsernameExists

    public async Task<bool> IsUsernameExistsAsync(string username, int? userId)
    {
        if (userId is not null)
        {
            return await _sliceCloudContext.Users.AnyAsync(u => u.UserName == username && u.UserId != userId);
        }
        return await _sliceCloudContext.Users.AnyAsync(u => u.UserName == username);
    }

    #endregion


    #region GetUserById

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _sliceCloudContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    #endregion

    #region GetUserByEmailAsync

    public async Task<User?> GetUserByEmailAsync(string userEmail)
    {
        return await _sliceCloudContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
    }

    #endregion

    #region CreateUser

    public async Task<bool> CreateUserAsync(User user)
    {
        _sliceCloudContext.Users.Add(user);
        return await _sliceCloudContext.SaveChangesAsync() > 0;
    }

    #endregion

    #region  

    public async Task<bool> UpdateUserAsync(User user)
    {

        _sliceCloudContext.Users.Update(user);

        UsersLogin? userLogin = await _sliceCloudContext.UsersLogins.FirstOrDefaultAsync(
            u => u.Email == user.Email
        );

        if (userLogin is not null)
        {
            userLogin.RoleId = user.RoleId;
            _sliceCloudContext.UsersLogins.Update(userLogin);
        }
        return await _sliceCloudContext.SaveChangesAsync() > 0;
    }

    #endregion

    #region DeleteExistingUser

    public async Task<bool> DeleteExistingUserAsync(int userId)
    {
        User? user = await _sliceCloudContext.Users.FindAsync(userId);

        if (user == null)
            return false;

        user.IsDeleted = true;
        _sliceCloudContext.Users.Update(user);

        return await _sliceCloudContext.SaveChangesAsync() > 0;
    }

    #endregion
}
