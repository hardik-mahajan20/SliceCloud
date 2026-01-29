using System.ComponentModel.DataAnnotations;
using SliceCloud.Repository.Enums;

namespace SliceCloud.Repository.ViewModels;

public class UserViewModel
{
    public int Id { get; set; }

    [Required, StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required, StringLength(30, ErrorMessage = "UserName cannot exceed 30 characters.")]
    public string UserName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    public int RoleId { get; set; }

    [Required]
    public int CountryId { get; set; }

    [Required]
    public int StateId { get; set; }

    [Required]
    public int CityId { get; set; }

    [StringLength(10, ErrorMessage = "ZipCode cannot exceed 10 characters.")]
    public string ZipCode { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
    public string Address { get; set; } = string.Empty;

    [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 digits.")]
    public string PhoneNumber { get; set; } = string.Empty;

    public UserRoles Roles { get; set; }
}
