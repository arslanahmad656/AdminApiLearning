using Aro.Admin.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Aro.Admin.Presentation.Entry.ContextFactory;

public class AroAdminApiDesignTimeDbContext : IDesignTimeDbContextFactory<AroAdminApiDbContext>
{
    public AroAdminApiDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<AroAdminApiDbContext>()
            .UseSqlServer(configuration.GetConnectionString("sqlConnection"), b => b.MigrationsAssembly(this.GetType().Assembly.GetName().Name));

        return new AroAdminApiDbContext(builder.Options);
    }
}
