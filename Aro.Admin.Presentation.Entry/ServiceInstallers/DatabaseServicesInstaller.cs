using Aro.Admin.Application.Services.Email;
using Aro.Admin.Application.Services.Migration;
using Aro.Admin.Application.Services.PermissionSeeder;
using Aro.Admin.Application.Services.Role;
using Aro.Admin.Application.Services.SystemSettings;
using Aro.Admin.Application.Services.User;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Infrastructure.Repository.Context;
using Aro.Admin.Infrastructure.Repository.Repositories;
using Aro.Admin.Infrastructure.Services;
using Aro.Common.Application.Services.Audit;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

internal class DatabaseServicesInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AroAdminApiDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection"), b => b.MigrationsAssembly(this.GetType().Assembly));
            var enableSensitiveData = builder.Configuration.GetValue<bool>("EnableEfCoreParameterValuesLogging");

            if (enableSensitiveData)
            {
                options.EnableSensitiveDataLogging(true);
            }
        });

        builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();

        // Add data services here
        builder.Services.AddScoped<IPermissionSeeder, PermissionSeeder>();
        builder.Services.AddScoped<IAuditService, AuditService>();
        builder.Services.AddScoped<IMigrationService, MigrationService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ISystemSettingsService, SystemSettingsService>();
        builder.Services.AddScoped<IRoleService, RoleService>();
        builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        builder.Services.AddScoped<IEmailTemplateSeeder, EmailTemplateSeeder>();
        builder.Services.AddScoped<IGroupService, GroupService>();
    }
}
