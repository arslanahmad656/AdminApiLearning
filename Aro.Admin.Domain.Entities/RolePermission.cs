namespace Aro.Admin.Domain.Entities;

public class RolePermission : IEntity
{
    public Guid RoleId { get; set; }
    public Role Role { get; set; }
    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; }
}
