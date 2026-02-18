namespace SliceCloud.Repository.Models;

public partial class Permission
{
    public int PermissionId { get; set; }

    public int RoleId { get; set; }

    public int ModuleId { get; set; }

    public bool? CanView { get; set; }

    public bool? CanAddEdit { get; set; }

    public bool? CanDelete { get; set; }

    public virtual PermissionModule Module { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
