using Aro.Admin.Application.Mediator.User.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using AutoMapper;
using MediatR;

namespace Aro.Admin.Application.Mediator.User.Handlers;

public class UserCreatedNotificationHandler(IAuditService auditService, IMapper mapper) : INotificationHandler<UserCreatedNotification>
{
    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogUserCreated(mapper.Map<UserCreatedLog>(notification.CreateUserResponse), cancellationToken).ConfigureAwait(false);
    }
}
