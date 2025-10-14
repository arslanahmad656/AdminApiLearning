using Aro.Admin.Application.Mediator.User.Commands;
using Aro.Admin.Application.Mediator.User.DTOs;
using Aro.Admin.Application.Mediator.User.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters;
using MediatR;

namespace Aro.Admin.Application.Mediator.User.Handlers;

public class CreateUserCommandHandler(IUserService userService, IMediator mediator) : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var response = await userService.CreateUser(new(request.CreateUserRequest.Email, request.CreateUserRequest.IsActive, false, request.CreateUserRequest.Password, request.CreateUserRequest.DisplayName, request.CreateUserRequest.AssignedRoles), cancellationToken).ConfigureAwait(false);

        var result = new CreateUserResponse(response.Id, response.Email, response.AssignedRoles);
        await mediator.Publish(new UserCreatedNotification(result), cancellationToken).ConfigureAwait(false);

        return result;
    }
}
