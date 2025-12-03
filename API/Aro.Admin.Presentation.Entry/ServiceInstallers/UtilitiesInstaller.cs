using Aro.Admin.Application.Services.DateFormatter;
using Aro.Admin.Application.Services.Hasher;
using Aro.Admin.Application.Services.RandomValueGenerator;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Infrastructure.Services;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Application.Services.RequestInterpretor;
using Aro.Common.Application.Services.Serializer;
using Aro.Common.Application.Services.UniqueIdGenerator;
using Aro.Common.Domain.Shared;
using Aro.Common.Infrastructure.Services;

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
        builder.Services.AddSingleton<IMultiFormatter, MultiFormatter>();
        builder.Services.AddSingleton<IRandomValueGenerator, SecureRandomStringGenerator>();
    }
}
