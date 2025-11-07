using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.Loader;

namespace Aro.Common.Infrastructure.Repository.Context;

public class AroDbContext : DbContext
{
    public AroDbContext()
    {
        
    }

    public AroDbContext(DbContextOptions<AroDbContext> options)
        : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
#if DEBUG
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
        }
#endif

        var assemblies = AssemblyLoadContext.Default.Assemblies
            .Where(a => !a.IsDynamic && (a.FullName ?? string.Empty).StartsWith("Aro."));

        foreach(var assembly in assemblies)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }

        base.OnModelCreating(modelBuilder);
    }
}
