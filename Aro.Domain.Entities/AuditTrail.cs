namespace Aro.Domain.Entities;

public class AuditTrail
{
    public Guid Id { get; set; }
    public Guid? ActorUserId { get; set; }
    public string ActorName { get; set; }
    public string Action { get; set; } // e.g. "User.Create"
    public string EntityType { get; set; }  // e.g. "user", "role"
    public string EntityId { get; set; }
    public string Before { get; set; }
    public string After { get; set; }
    public string IpAddress { get; set; }
    public DateTime Timestamp { get; set; }
}
