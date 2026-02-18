namespace SliceCloud.Repository.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<UsersLogin> UsersLogins { get; set; } = new List<UsersLogin>();
}
