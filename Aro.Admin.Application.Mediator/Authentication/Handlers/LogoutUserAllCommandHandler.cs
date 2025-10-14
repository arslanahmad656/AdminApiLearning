using Aro.Admin.Application.Mediator.Authentication.Commands;
using Aro.Admin.Application.Mediator.Authentication.DTOs;
using Aro.Admin.Application.Mediator.Authentication.Notifications;
using Aro.Admin.Application.Services;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Handlers;

public class LogoutUserAllCommandHandler(IAuthenticationService authenticationService, IMediator mediator) : IRequestHandler<LogoutUserAllCommand>
{
    public async Task Handle(LogoutUserAllCommand request, CancellationToken cancellationToken)
    {
        await authenticationService.LogoutAll(request.Data.UserId, cancellationToken).ConfigureAwait(false);

        await mediator.Publish(new UserLoggedOutAllNotification(new UserLoggedOutAllNotificationData(request.Data.UserId)), cancellationToken).ConfigureAwait(false);
    }
}
