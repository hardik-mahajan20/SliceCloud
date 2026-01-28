using System.ComponentModel.DataAnnotations;
using SliceCloud.Repository.Enums;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.ViewModels;

public class MyProfileViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "First Name is required.")]
    [StringLength(20, ErrorMessage = "First Name cannot exceed 20 characters.")]
    [RegularExpression(@"^[A-Za-z\s]{2,50}$", ErrorMessage = "Name must contain only letters and be between 2 and 50 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last Name is required.")]
    [StringLength(20, ErrorMessage = "Last Name cannot exceed 20 characters.")]
    [RegularExpression(@"^[A-Za-z\s]{2,50}$", ErrorMessage = "Last Name must contain only letters and be between 2 and 50 characters.")]
    public required string LastName { get; set; } = string.Empty;

    public string? Email { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number format.")]
    public required string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
    public required string Address { get; set; }

    public string? ProfileImage { get; set; }

    public int? UserId { get; set; }

    [Required(ErrorMessage = "Please select a valid country.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a country.")]
    public int CountryId { get; set; }

    [Required(ErrorMessage = "Please select a valid State.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a state.")]
    public int StateId { get; set; }

    [Required(ErrorMessage = "Please select a valid City.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a city.")]
    public int CityId { get; set; }

    [Required(ErrorMessage = "Zipcode is required.")]
    [RegularExpression(@"^\d{5,10}$", ErrorMessage = "Invalid Zipcode format.")]
    public string? ZipCode { get; set; }

    [Required(ErrorMessage = "Username  is required.")]
    [StringLength(20, ErrorMessage = "Username cannot exceed 20 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9_]{3,20}$", ErrorMessage = "Username can only contain letters, numbers, and underscores, and must be between 3-20 characters.")]
    public required string UserName { get; set; }

    public UserRoles Role { get; set; }

    public List<Country>? Countries { get; set; }

    public List<State>? States { get; set; }

    public List<City>? Cities { get; set; }
}

