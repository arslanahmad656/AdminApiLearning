namespace Aro.Admin.Domain.Entities;

public class Permission : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }      // eg. "users.create"
    public string Resource { get; set; }  // eg. "users"
    public string Action { get; set; }    // eg. "create", "update"
    public ICollection<RolePermission> RolePermissions { get; set; }
}
