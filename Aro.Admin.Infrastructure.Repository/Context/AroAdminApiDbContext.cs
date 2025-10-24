using Aro.Admin.Domain.Entities;
using Aro.Admin.Infrastructure.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Aro.Admin.Infrastructure.Repository.Context;

public class AroAdminApiDbContext : DbContext
{
    public AroAdminApiDbContext()
    {
        
    }

    public AroAdminApiDbContext(DbContextOptions<AroAdminApiDbContext> options) : base(options)
    {
        
    }

    public DbSet<AuditTrail> AuditTrails { get; set; }

    public DbSet<IdempotencyRecord> IdempotencyRecords { get; set; }

    public DbSet<Permission> Permissions { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<RolePermission> RolePermissions { get; set; }

    public DbSet<SystemSetting> SystemSettings { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<UserRole> UserRoles { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<EmailTemplate> EmailTemplates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
#if DEBUG
        if (!Debugger.IsAttached)
        {
            //Debugger.Launch();
        }
#endif

        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
