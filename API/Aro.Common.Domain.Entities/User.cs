namespace Aro.Common.Domain.Entities;

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
    public ContactInfo ContactInfo { get; set; }

    // Account lockout properties
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockoutEnd { get; set; }
}
