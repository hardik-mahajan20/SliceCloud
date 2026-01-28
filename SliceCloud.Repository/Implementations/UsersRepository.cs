using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Repository.Implementations;

public class UsersRepository(SliceCloudContext sliceCloudContext) : IUsersRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllUsers

    public async Task<PaginatedList<User>> GetAllUsersAsync(int pageNumber, int pageSize, string query, string sortOrder, string sortColumn, string search)
    {
        IQueryable<User>? usersQuery = _sliceCloudContext.Users
       .AsNoTracking()
       .Where(u => u.IsDeleted == false);

        if (!string.IsNullOrEmpty(search))
        {
            usersQuery = usersQuery.Where(u =>
                (u.FirstName != null && (u.FirstName.Contains(search) || u.FirstName.Contains(search, StringComparison.CurrentCultureIgnoreCase))) ||
                (u.Email != null && (u.Email.Contains(search) || u.Email.Contains(search, StringComparison.CurrentCultureIgnoreCase))) ||
                (u.PhoneNumber != null && (u.PhoneNumber.Contains(search) || u.PhoneNumber.Contains(search, StringComparison.CurrentCultureIgnoreCase)))
            );
        }

        if (!string.IsNullOrEmpty(sortColumn))
        {
            ParameterExpression parameter = Expression.Parameter(typeof(User), "u");
            MemberExpression property = Expression.Property(parameter, sortColumn);
            Expression<Func<User, object>> lambda = Expression.Lambda<Func<User,
              object>>(Expression.Convert(property, typeof(object)), parameter);

            usersQuery = sortOrder.Equals("desc", StringComparison.CurrentCultureIgnoreCase) ?
              usersQuery.OrderByDescending(lambda) :
              usersQuery.OrderBy(lambda);
        }

        PaginatedList<User> paginatedUsers = await PaginatedList<User>.CreateAsync(usersQuery, pageNumber, pageSize);
        return paginatedUsers;
    }

    #endregion

    #region IsEmailExists

    public async Task<bool> IsEmailExistsAsync(string email, int? userId)
    {
        if (userId is not null)
        {
            return await _sliceCloudContext.Users.AnyAsync(u => u.Email == email && u.UserId != userId);
        }
        else
        {
            return await _sliceCloudContext.Users.AnyAsync(u => u.Email == email);
        }
    }

    #endregion

    #region IsPhoneExists

    public async Task<bool> IsPhoneExistsAsync(string phone, int? userId)
    {
        if (userId is not null)
        {
            return await _sliceCloudContext.Users.AnyAsync(u => u.PhoneNumber == phone && u.UserId != userId);
        }
        else
        {
            return await _sliceCloudContext.Users.AnyAsync(u => u.PhoneNumber == phone);
        }
    }
    #endregion

    #region IsUsernameExists

    public async Task<bool> IsUsernameExistsAsync(string username, int? userId)
    {
        if (userId is not null)
        {
            return await _sliceCloudContext.Users.AnyAsync(u => u.UserName == username && u.UserId != userId);
        }
        else
        {
            return await _sliceCloudContext.Users.AnyAsync(u => u.UserName == username);
        }
    }

    #endregion

    #region CreateUser

    public async Task<bool> CreateUserAsync(User user)
    {
        if (user != null)
        {
            _sliceCloudContext.Users.Add(user);
            await _sliceCloudContext.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region GetUserById

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _sliceCloudContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    #endregion

    #region  

    public async Task<bool> UpdateUserAsync(User user)
    {
        if (user == null)
        {
            return false;
        }

        _sliceCloudContext.Users.Update(user);

        UsersLogin? userLogin = await _sliceCloudContext.UsersLogins.FirstOrDefaultAsync(
            u => u.Email == user.Email
        );

        if (userLogin is not null)
        {
            userLogin.RoleId = user.RoleId;
            _sliceCloudContext.UsersLogins.Update(userLogin);
        }
        int rowAffected = await _sliceCloudContext.SaveChangesAsync();
        return rowAffected > 0;
    }

    #endregion

    #region DeleteExistingUser

    public async Task<bool> DeleteExistingUserAsync(int userId)
    {
        User? user = await _sliceCloudContext.Users.FindAsync(userId);
        if (user == null)
            return false;

        User? newUser = user;
        if (user.IsDeleted == false)
        {
            user.IsDeleted = true;
        }
        _sliceCloudContext.Users.Update(user);

        return await _sliceCloudContext.SaveChangesAsync() > 0;
    }

    #endregion
}
