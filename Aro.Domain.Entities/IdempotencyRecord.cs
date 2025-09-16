namespace Aro.Domain.Entities;

public class IdempotencyRecord
{
    public string Key { get; set; }
    public string ResponseData { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public Guid? UserId { get; set; }
}
