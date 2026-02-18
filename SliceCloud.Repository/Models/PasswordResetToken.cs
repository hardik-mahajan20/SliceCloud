namespace SliceCloud.Repository.Models;

public partial class PasswordResetToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiryTime { get; set; }

    public virtual UsersLogin User { get; set; } = null!;
}
