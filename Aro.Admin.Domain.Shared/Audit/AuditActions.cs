namespace Aro.Admin.Domain.Shared.Audit;

public class AuditActions
{
    public readonly string ApplicationSeeded = "APPLICATION_SEEDED";
    public readonly string MigrationsApplied = "MIGRATIONS_APPLIED";
    public readonly string SystemInitialized = "SYSTEM_INITIALIZED";
    public readonly string UserCreated = "USER_CREATED";
    public readonly string RolesAssignedToUsers = "ROLES_ASSIGNED_TO_USERS";
    public readonly string RolesRevokedFromUsers = "ROLES_REVOKED_FROM_USERS";
}
