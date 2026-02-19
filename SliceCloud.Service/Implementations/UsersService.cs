using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Repository.Enums;
using SliceCloud.Service.Interfaces;
using SliceCloud.Service.Utils;
using Microsoft.EntityFrameworkCore;

namespace SliceCloud.Service.Implementations;

public class UsersService(IUsersRepository usersRepository, IRolesRepository rolesRepository, IUsersLoginService usersLoginService, IImageService imageService, IWebHostEnvironment env) : IUsersService
{
    private readonly IUsersRepository _usersRepository = usersRepository;
    private readonly IRolesRepository _rolesRepository = rolesRepository;
    private readonly IUsersLoginService _usersLoginService = usersLoginService;

    private readonly IImageService _imageService = imageService;

    private readonly IWebHostEnvironment _env = env;

    #region GetAllUsers

    public async Task<PaginatedList<User>> GetAllUsersAsync(int pageNumber, int pageSize, string query, string sortOrder, string sortColumn, string search)
    {
        PaginatedList<User> paginatedUsers = await _usersRepository.GetAllUsersAsync(pageNumber, pageSize, query, sortOrder, sortColumn, search);

        foreach (var user in paginatedUsers)
        {
            user.ProfileImage = string.IsNullOrEmpty(user.ProfileImage)
                ? "images/uploads/default_pfp.svg"
                : "images/uploads/" + user.ProfileImage;
        }

        return paginatedUsers;
    }

    #endregion

    #region ValidateUniqueFields

    public async Task<Dictionary<string, string>> ValidateUniqueFieldsAsync(CreateUserViewModel createUserViewModel)
    {
        Dictionary<string, string> errors = [];

        if (createUserViewModel.Email != null)
            if (await _usersRepository.IsEmailExistsAsync(createUserViewModel.Email, null))
                errors.Add(nameof(createUserViewModel.Email), "Email already exists.");

        if (await _usersRepository.IsUsernameExistsAsync(createUserViewModel.UserName, null))
            errors.Add(nameof(createUserViewModel.UserName), "UserName already exists.");

        if (createUserViewModel.Phone != null)
            if (await _usersRepository.IsPhoneExistsAsync(createUserViewModel.Phone, null))
                errors.Add(nameof(createUserViewModel.Phone), "Phone number already exists.");

        return errors;
    }

    #endregion

    #region ValidateUniqueFields

    public async Task<Dictionary<string, string>> ValidateUniqueFieldsAsync(UpdateUserViewModel updateUserViewModel)
    {
        Dictionary<string, string> errors = [];

        if (updateUserViewModel.Email != null)
            if (await _usersRepository.IsEmailExistsAsync(updateUserViewModel.Email, updateUserViewModel.Id))
                errors.Add(nameof(updateUserViewModel.Email), "Email already exists.");

        if (await _usersRepository.IsUsernameExistsAsync(updateUserViewModel.UserName, updateUserViewModel.Id))
            errors.Add(nameof(updateUserViewModel.UserName), "UserName already exists.");

        if (updateUserViewModel.PhoneNumber != null)
            if (await _usersRepository.IsPhoneExistsAsync(updateUserViewModel.PhoneNumber, updateUserViewModel.Id))
                errors.Add(nameof(updateUserViewModel.PhoneNumber), "Phone number already exists.");

        return errors;
    }

    #endregion

    #region CreateUser

    public async Task<bool> CreateUserAsync(CreateUserViewModel createUserViewModel, IFormFile itemImage)
    {
        Role? role = await _rolesRepository.GetRoleByIdAsync(createUserViewModel.RoleId);

        if (createUserViewModel.Role == null && role!.RoleName != null)
        {
            createUserViewModel.Role = role!.RoleName.ToString();
        }

        string? itemImgPath = await _imageService.ImgPath(itemImage);

        User user = new()
        {
            FirstName = createUserViewModel.FirstName,
            LastName = createUserViewModel.LastName,
            UserName = createUserViewModel.UserName,
            RoleId = createUserViewModel.RoleId,
            Status = 1,
            Email = createUserViewModel.Email,
            PasswordHash = PasswordUtils.HashPassword(createUserViewModel.Password!),
            ZipCode = createUserViewModel.ZipCode,
            Address = createUserViewModel.Address,
            PhoneNumber = createUserViewModel.Phone,
            CountryId = createUserViewModel.CountryId,
            StateId = createUserViewModel.StateId,
            CityId = createUserViewModel.CityId,
            ProfileImage = itemImgPath
        };

        bool isUserCreated = await _usersRepository.CreateUserAsync(user);
        if (isUserCreated)
        {
            int userId = user.UserId;
            UsersLoginViewModel login = new()
            {
                Email = user.Email,
                UserId = userId,
                UserName = user.UserName,
                HashPassword = user.PasswordHash,
                RoleId = user.RoleId,
                Status = (UserStatus)1,
            };

            await _usersLoginService.CreateUserLoginAsync(login);
            return true;
        }
        return false;
    }

    #endregion

    #region GetUserById

    public async Task<UpdateUserViewModel?> GetUserByIdAsync(int id)
    {
        User? user = await _usersRepository.GetUserByIdAsync(id);

        if (user != null)
        {

            user.ProfileImage = string.IsNullOrEmpty(user.ProfileImage)
               ? ""
               : "images/uploads/" + user.ProfileImage;
            UpdateUserViewModel updateUserViewModel = new()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName!,
                RoleId = user.RoleId,
                Email = user.Email!,
                Password = user.PasswordHash!,
                CountryId = user.CountryId,
                StateId = user.StateId,
                CityId = user.CityId,
                Address = user.Address!,
                ZipCode = user.ZipCode!,
                PhoneNumber = user.PhoneNumber,
                Status = user.Status.HasValue ? (UserStatus)user.Status : UserStatus.Active,
                ProfileImage = user.ProfileImage!
            };

            return updateUserViewModel;
        }
        else
        {
            return null;
        }
    }

    #endregion UpdateExitingUser

    #region 

    public async Task<bool> UpdateExitingUserAsync(UpdateUserViewModel updateUserViewModel, int id, IFormFile itemImage)
    {
        User? user = await _usersRepository.GetUserByIdAsync(id);

        if (itemImage != null)
        {
            user!.ProfileImage = await _imageService.ImgPath(itemImage);
        }
        else
        {
            user!.ProfileImage = updateUserViewModel.ProfileImage;
        }


        if (user != null)
        {
            user.FirstName = updateUserViewModel.FirstName;
            user.LastName = updateUserViewModel.LastName;
            user.UserName = updateUserViewModel.UserName;
            user.RoleId = updateUserViewModel.RoleId;
            user.Email = user.Email;
            user.Status = (int?)updateUserViewModel.Status;
            user.CountryId = updateUserViewModel.CountryId;
            user.StateId = updateUserViewModel.StateId;
            user.CityId = updateUserViewModel.CityId;
            user.Address = updateUserViewModel.Address;
            user.ZipCode = updateUserViewModel.ZipCode;
            user.PhoneNumber = updateUserViewModel.PhoneNumber;

            await _usersRepository.UpdateUserAsync(user);

            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region DeleteExistingUser

    public async Task<bool> DeleteExistingUserAsync(int id)
    {
        return await _usersRepository.DeleteExistingUserAsync(id);
    }

    #endregion

    #region GetAllowedRoles

    public async Task<List<Role>> GetAllowedRolesAsync(ClaimsPrincipal user)
    {
        string? userRole = user.FindFirst(ClaimTypes.Role)?.Value;
        HashSet<int> excludedRoleIds = [];

        if (userRole == "Manager")
        {
            excludedRoleIds.Add(1);
        }
        else if (userRole == "Chef")
        {
            excludedRoleIds.Add(1);
            excludedRoleIds.Add(2);
        }

        List<Role>? allRoles = await _rolesRepository.GetAllRolesAsQueryable().ToListAsync();

        return allRoles
            .Where(r => !excludedRoleIds.Contains(r.RoleId))
            .ToList();
    }



    #region DeleteProfileImage

    public bool DeleteProfileImage(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            return false;

        // Remove leading slash if exists
        imagePath = imagePath.TrimStart('/');

        string fullPath = Path.Combine(
            _env.WebRootPath,
            imagePath
        );

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return true;
        }

        return false;
    }

    #endregion


    #endregion
}
