using Aro.Admin.Application.Mediator.User.Commands;
using Aro.Admin.Application.Mediator.User.Notifications;
using Aro.Admin.Application.Services.User;
using MediatR;

namespace Aro.Admin.Application.Mediator.User.Handlers;

public class ChangePasswordCommandHandler(IUserService userService, IMediator mediator) : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
		try
		{
            await userService.ChangePassword(new(request.Request.Email, request.Request.OldPassword, request.Request.NewPassword), cancellationToken).ConfigureAwait(false);
            await mediator.Publish(new ChangePasswordSuccessNotification(new(request.Request.Email)), cancellationToken).ConfigureAwait(false);
        }
		catch (Exception ex)
		{
			await mediator.Publish(new ChangePasswordFailedNotification(new(request.Request.Email, ex.Message)), cancellationToken).ConfigureAwait(false);
			throw;
		}
    }
}
