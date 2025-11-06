using Aro.Admin.Application.Services.DateFormatter;
using Aro.Admin.Application.Services.Hasher;
using Aro.Admin.Application.Services.RandomValueGenerator;
using Aro.Admin.Application.Services.RequestInterpretor;
using Aro.Admin.Application.Services.Serializer;
using Aro.Admin.Application.Services.UniqueIdGenerator;
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
        builder.Services.AddSingleton<IUniqueIdGenerator, GuidGenerator>();
        builder.Services.AddSingleton<IHasher, BCryptHasher>();
        builder.Services.AddSingleton<ErrorCodes>();
        builder.Services.AddSingleton<AuditActions>();
        builder.Services.AddSingleton<EntityTypes>();
        builder.Services.AddSingleton<SharedKeys>();
        builder.Services.AddSingleton<IDateFormatter, ISO8601DateFormatter>();
        builder.Services.AddSingleton<IRandomValueGenerator, SecureRandomStringGenerator>();
    }
}
