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
        await auditService.LogAuthenticationSuccessful(mapper.Map<AuthenticationSuccessfulLog>(notification.Data), cancellationToken).ConfigureAwait(false);
    }
}
