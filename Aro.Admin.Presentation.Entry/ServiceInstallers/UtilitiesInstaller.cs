
using Aro.Admin.Application.Services;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Domain.Shared.Audit;
using Aro.Admin.Domain.Shared.Exceptions;
using Aro.Admin.Infrastructure.Services;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

internal class UtilitiesInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IRequestInterpretorService, RequestInterpretorService>();
        builder.Services.AddSingleton<ISerializer, JsonSerializer>();
        builder.Services.AddSingleton<IEntityIdGenerator, GuidEntityIdGenerator>();
        builder.Services.AddSingleton<IPasswordHasher, BCrypttPasswordHasher>();
        builder.Services.AddSingleton<ErrorCodes>();
        builder.Services.AddSingleton<AuditActions>();
        builder.Services.AddSingleton<AuditEntityTypes>();
        builder.Services.AddSingleton<SharedKeys>();
        builder.Services.AddSingleton<PermissionCodes>();
    }
}
