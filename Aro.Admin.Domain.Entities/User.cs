namespace Aro.Admin.Domain.Entities;

public class User : IEntity
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string DisplayName { get; set; }
    public bool IsSystem { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; }
}
