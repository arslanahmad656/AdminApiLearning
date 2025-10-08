using Aro.Admin.Application.Mediator.Authentication.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using AutoMapper;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Handlers;

public class UserAuthenticationFailedNotificationHandler(IAuditService auditService, IMapper mapper) : INotificationHandler<UserAuthenticationFailedNotification>
{
    public async Task Handle(UserAuthenticationFailedNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogAuthenticationFailed(mapper.Map<AuthenticationFailedLog>(notification.Data), cancellationToken).ConfigureAwait(false);
    }
}
