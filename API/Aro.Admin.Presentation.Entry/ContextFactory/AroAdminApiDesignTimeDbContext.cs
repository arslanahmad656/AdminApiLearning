using Aro.Common.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Aro.Admin.Presentation.Entry.ContextFactory;

public class AroAdminApiDesignTimeDbContext : IDesignTimeDbContextFactory<AroDbContext>
{
    public AroDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<AroDbContext>()
            .UseSqlServer(configuration.GetConnectionString("sqlConnection"), b => b.MigrationsAssembly(this.GetType().Assembly.GetName().Name));

        return new AroDbContext(builder.Options);
    }
}
