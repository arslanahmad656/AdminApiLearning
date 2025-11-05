using Aro.Common.Domain.Entities;

namespace Aro.Admin.Domain.Entities;

public class Permission : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }      // eg. "users.create"
    public ICollection<RolePermission> RolePermissions { get; set; }
}
