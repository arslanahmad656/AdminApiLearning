using Aro.Common.Domain.Entities;

namespace Aro.Admin.Domain.Entities;

public class IdempotencyRecord : IEntity
{
    public string Key { get; set; }
    public string ResponseData { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public Guid? UserId { get; set; }
}
