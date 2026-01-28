namespace SliceCloud.Repository.ViewModels;

public class UsersLoginViewModel
{
    public string Email { get; set; } = string.Empty;

    public string HashPassword { get; set; } = string.Empty;

    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public Status Status { get; set; }
}
public enum Status
{
    Active = 1,
    InActive = 2
}
