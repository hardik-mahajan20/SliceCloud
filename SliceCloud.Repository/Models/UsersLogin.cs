namespace SliceCloud.Repository.Models;

public partial class UsersLogin
{
    public int UserLoginId { get; set; }

    public int? UserId { get; set; }

    public int RoleId { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public string? RefreshToken { get; set; }

    public string? ResetToken { get; set; }

    public DateTime? ResetTokenExpiration { get; set; }

    public bool IsFirstLogin { get; set; }

    public bool? IsResetTokenUsed { get; set; }

    public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();

    public virtual Role Role { get; set; } = null!;

    public virtual User? User { get; set; }
}
