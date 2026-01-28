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
}
