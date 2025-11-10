using Aro.Admin.Application.Services.Email;
using Aro.Admin.Application.Services.Migration;
using Aro.Admin.Application.Services.PermissionSeeder;
using Aro.Admin.Application.Services.Role;
using Aro.Admin.Application.Services.SystemSettings;
using Aro.Admin.Application.Services.User;
using Aro.Admin.Infrastructure.Repository;
using Aro.Admin.Infrastructure.Services;
using Aro.Booking.Application.Services.Group;
using Aro.Booking.Infrastructure.Services;
using Aro.Common.Application.Repository;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Infrastructure.Repository;
using Aro.Common.Infrastructure.Repository.Context;
using Aro.Common.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

internal class DatabaseServicesInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AroDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection"), b => b.MigrationsAssembly(this.GetType().Assembly));
            var enableSensitiveData = builder.Configuration.GetValue<bool>("EnableEfCoreParameterValuesLogging");

            if (enableSensitiveData)
            {
                options.EnableSensitiveDataLogging(true);
            }
        });

        builder.Services.AddScoped<Common.Application.Repository.IRepositoryManager, Common.Infrastructure.Repository.RepositoryManager>();
        builder.Services.AddScoped<Booking.Application.Repository.IRepositoryManager, Booking.Infrastructure.Repository.RepositoryManager>();
        builder.Services.AddScoped<Admin.Application.Repository.IRepositoryManager, Admin.Infrastructure.Repository.RepositoryManager>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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
