using Aro.Common.Infrastructure.Shared;
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

        var basePath = AppContext.BaseDirectory;
        var configurationDlls = new EntityConfigurationDlls().Dlls;

        foreach (var dllPath in configurationDlls)
        {
            var fullPath = Path.Combine(basePath, dllPath);
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(fullPath);
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }

        base.OnModelCreating(modelBuilder);
    }
}
