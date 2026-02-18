namespace SliceCloud.Repository.Models;

public partial class User
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? ProfileImage { get; set; }

    public string? Address { get; set; }

    public string? ZipCode { get; set; }

    public int CountryId { get; set; }

    public int StateId { get; set; }

    public int CityId { get; set; }

    public int? Status { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual State State { get; set; } = null!;

    public virtual ICollection<UsersLogin> UsersLogins { get; set; } = new List<UsersLogin>();
}
