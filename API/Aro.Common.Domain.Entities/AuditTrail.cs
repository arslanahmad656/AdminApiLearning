namespace Aro.Common.Domain.Entities;

public class AuditTrail : IEntity
{
    public Guid Id { get; set; }
    public Guid? ActorUserId { get; set; }  // FK to user (1 - N)
    public string ActorName { get; set; }
    public string Action { get; set; } // e.g. "User.Create"
    public string EntityType { get; set; }  // e.g. "user", "role"
    public string EntityId { get; set; }
    public string Data { get; set; }
    public string IpAddress { get; set; }
    public DateTime Timestamp { get; set; }
}
