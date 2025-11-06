using Aro.Admin.Application.Mediator.SystemSettings.Queries;
using Aro.Admin.Application.Services.SystemSettings;
using MediatR;

namespace Aro.Admin.Application.Mediator.SystemSettings.Handlers;

public class IsSystemInitializedQueryHandler(ISystemSettingsService systemSettingsService) : IRequestHandler<IsSystemInitializedQuery, bool>
{
    public async Task<bool> Handle(IsSystemInitializedQuery request, CancellationToken cancellationToken)
    {
        var isInitialized = await systemSettingsService.IsSystemInitialized(cancellationToken).ConfigureAwait(false);

        return isInitialized;
    }
}
