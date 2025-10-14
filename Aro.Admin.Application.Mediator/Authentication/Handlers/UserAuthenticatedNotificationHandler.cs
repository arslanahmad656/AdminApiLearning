using Aro.Admin.Application.Mediator.Authentication.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using AutoMapper;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Handlers;

public class UserAuthenticatedNotificationHandler(IAuditService auditService, IMapper mapper) : INotificationHandler<UserAuthenticatedNotification>
{
    public async Task Handle(UserAuthenticatedNotification notification, CancellationToken cancellationToken)
    {
        var data = mapper.Map<AuthenticationSuccessfulLog>(notification.Data);
        await auditService.LogAuthenticationSuccessful(data, cancellationToken).ConfigureAwait(false);
    }
}
