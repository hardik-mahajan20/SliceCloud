using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Service.Interfaces;

public interface IUsersService
{
    /// <summary>
    /// Retrieves all users as an paginated result.
    /// </summary>
    /// <returns>An paginated result of all users.</returns>
    Task<PaginatedList<User>> GetAllUsersAsync(int pageNumber, int pageSize, string query, string sortOrder, string sortColumn, string search);

    /// <summary>
    /// Validates the uniqueness of fields for a new user asynchronously.
    /// </summary>
    /// <param name="model">The view model containing user details.</param>
    /// <returns>A task that returns a dictionary of validation errors, if any.</returns>
    Task<Dictionary<string, string>> ValidateUniqueFieldsAsync(CreateUserViewModel createUserViewModel);

    /// <summary>
    /// Validates the uniqueness of fields for a new user asynchronously.
    /// </summary>
    /// <param name="model">The view model containing user details.</param>
    /// <returns>A task that returns a dictionary of validation errors, if any.</returns>
    Task<Dictionary<string, string>> ValidateUniqueFieldsAsync(UpdateUserViewModel updateUserViewModel);

    /// <summary>
    /// Creates a new user asynchronously.
    /// </summary>
    /// <param name="createUserViewModel">The view model containing user details.</param>
    /// <param name="itemImage">The form file for the image.</param>
    /// <returns>A task that returns true if the creation was successful, otherwise false.</returns>
    Task<bool> CreateUserAsync(CreateUserViewModel createUserViewModel, IFormFile itemImage);

    /// <summary>
    /// Retrieves a user by their ID asynchronously.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <returns>A task that returns the user view model if found, otherwise null.</returns>
    Task<UpdateUserViewModel?> GetUserByIdAsync(int userId);

    /// <summary>
    /// Updates an existing user asynchronously.
    /// </summary>
    /// <param name="updateUserViewModel">The view model containing updated user details.</param>
    /// <param name="id">The ID of the user to update.</param>
    /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
    Task<bool> UpdateExitingUserAsync(UpdateUserViewModel updateUserViewModel, int id, IFormFile itemImage);

    /// <summary>
    /// Deletes an existing user by their ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
    Task<bool> DeleteExistingUserAsync(int id);

    /// <summary>
    /// Gives allows list of roles for the particular user.
    /// </summary>
    /// <param name="claimsPrincipal">THe claimsPrincipal of the user.</param>
    /// <returns>A task that returns list of role a user allow to add.</returns>
    Task<List<Role>> GetAllowedRolesAsync(ClaimsPrincipal claimsPrincipal);
}
