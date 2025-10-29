namespace Aro.Admin.Domain.Entities;

public class PasswordResetToken : IEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime Expiry { get; set; }
    public bool IsUsed { get; set; }
    public string RequestIP { get; set; }
    public string UserAgent { get; set; }
    
    public User User { get; set; }
}
