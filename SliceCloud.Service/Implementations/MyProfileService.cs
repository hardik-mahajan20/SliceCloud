using SliceCloud.Repository.Enums;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class MyProfileService(IUsersRepository usersRepository) : IMyProfileService
{
    private readonly IUsersRepository _usersRepository = usersRepository;

    #region GetProfileById

    public async Task<MyProfileViewModel?> GetProfileByIdAsync(int userId)
    {
        User? user = await _usersRepository.GetUserByIdAsync(userId);

        if (user == null) return null;

        return new MyProfileViewModel
        {
            Id = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            CountryId = user.CountryId,
            StateId = user.StateId,
            CityId = user.CityId,
            Address = user.Address ?? string.Empty,
            ZipCode = user.ZipCode,
            ProfileImage = user.ProfileImage,
            Role = (UserRoles)user.RoleId,
            Email = user.Email
        };
    }

    #endregion

    #region IsUsernameTaken

    public async Task<bool> IsUsernameTakenAsync(string username, int currentUserId)
    {
        bool existingUser = await _usersRepository.IsUsernameExistsAsync(username, currentUserId);
        return existingUser;
    }

    #endregion

    #region UpdateProfile

    public async Task<MyProfileViewModel?> UpdateProfileAsync(int userId, MyProfileViewModel updateProfileViewModel)
    {
        User? user = await _usersRepository.GetUserByIdAsync(userId);
        if (user == null) return null;

        user.FirstName = updateProfileViewModel.FirstName;
        user.LastName = updateProfileViewModel.LastName;
        user.UserName = updateProfileViewModel.UserName;
        user.PhoneNumber = updateProfileViewModel.PhoneNumber;
        user.CountryId = updateProfileViewModel.CountryId;
        user.StateId = updateProfileViewModel.StateId;
        user.CityId = updateProfileViewModel.CityId;
        user.Address = updateProfileViewModel.Address;
        user.ZipCode = updateProfileViewModel.ZipCode;
        await _usersRepository.UpdateUserAsync(user);
        return await GetProfileByIdAsync(userId);
    }

    #endregion
}
