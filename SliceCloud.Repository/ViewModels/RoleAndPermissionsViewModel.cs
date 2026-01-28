namespace SliceCloud.Repository.ViewModels;

public class RoleAndPermissionsViewModel
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string UserRole { get; set; } = null!;

    public List<Permissions>? Permissions { get; set; }
}
public class Permissions
{
    public int PermissionId { get; set; }

    public int ModuleId { get; set; }

    public string ModuleName { get; set; } = string.Empty;

    public bool CanView { get; set; }

    public bool CanAddEdit { get; set; }

    public bool CanDelete { get; set; }

}
