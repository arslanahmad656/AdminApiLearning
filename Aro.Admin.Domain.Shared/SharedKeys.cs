namespace Aro.Admin.Domain.Shared;

public class SharedKeys
{
    public readonly string IS_SYSTEM_INITIALIZED = nameof(IS_SYSTEM_INITIALIZED);
    
    public readonly string IS_MIGRATIONS_COMPLETE = nameof(IS_MIGRATIONS_COMPLETE);

    public readonly string IS_DATABASE_SEEDED_AT_STARTUP = nameof(IS_DATABASE_SEEDED_AT_STARTUP);

    public readonly string JWT_CLAIM_ACTIVE = "active";
}
