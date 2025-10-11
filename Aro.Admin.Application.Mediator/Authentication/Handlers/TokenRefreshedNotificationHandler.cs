using Aro.Admin.Application.Mediator.Authentication.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using AutoMapper;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Handlers;

public class TokenRefreshedNotificationHandler(IAuditService auditService, IMapper mapper) : INotificationHandler<TokenRefreshedNotification>
{
    public async Task Handle(TokenRefreshedNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogTokenRefreshedLog(mapper.Map<TokenRefreshedLog>(notification.Data), cancellationToken).ConfigureAwait(false);
    }
}
