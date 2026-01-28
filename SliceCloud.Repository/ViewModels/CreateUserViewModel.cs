using System.ComponentModel.DataAnnotations;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.ViewModels;

public class CreateUserViewModel
{
    [Required(ErrorMessage = "First Name is required.")]
    [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
    [RegularExpression(@"^[A-Za-z\s]{2,50}$", ErrorMessage = "Name must contain only letters and be between 2 and 50 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last Name is required.")]
    [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
    [RegularExpression(@"^[A-Za-z\s]{2,50}$", ErrorMessage = "Last Name must contain only letters and be between 2 and 50 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "UserName  is required.")]
    [StringLength(20, ErrorMessage = "UserName cannot exceed 20 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9_]{3,20}$", ErrorMessage = "UserName can only contain letters, numbers, and underscores, and must be between 3-20 characters.")]
    public string UserName { get; set; } = string.Empty;

    [RegularExpression(@"^(Admin|User|Manager|Employee)$", ErrorMessage = "Please select any one role.")]
    public string Role { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
    [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid Email Address format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
    ErrorMessage = "Password must be at least 8 characters long, contain one uppercase, one lowercase, one digit, and one special character.")]
    public string Password { get; set; } = string.Empty;

    public string ProfileImage { get; set; } = string.Empty;

    [Required(ErrorMessage = "Zipcode is required.")]
    [RegularExpression(@"^\d{5,10}$", ErrorMessage = "Invalid Zipcode format.")]
    public string ZipCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required.")]
    [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number format.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a valid country.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid country.")]
    public int CountryId { get; set; }

    [Required(ErrorMessage = "Please select a valid State.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid state.")]
    public int StateId { get; set; }

    [Required(ErrorMessage = "Please select a valid Role.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Role.")]
    public int RoleId { get; set; }

    public int CityId { get; set; }

    public List<Country> Countries { get; set; } = [];

    public List<State> States { get; set; } = [];

    public List<City> Cities { get; set; } = [];

    public List<Role> Roles { get; set; } = [];
}

