namespace Aro.Admin.Domain.Entities;

public class PasswordHistory : IEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string PasswordHash { get; set; }

    public DateTime PasswordSetDate { get; set; }

    public User User { get; set; }
}
