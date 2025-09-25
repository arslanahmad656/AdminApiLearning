using Aro.Admin.Application.Mediator.User.Commands;
using Aro.Admin.Application.Mediator.User.DTOs;
using Aro.Admin.Application.Mediator.User.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters;
using AutoMapper;
using MediatR;

namespace Aro.Admin.Application.Mediator.User.Handlers;

public class CreateUserCommandHandler(IUserService userService, IMapper mapper, IMediator mediator) : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var response = await userService.CreateUser(mapper.Map<CreateUserDto>(request.CreateUserRequest), cancellationToken).ConfigureAwait(false);

        var result = mapper.Map<CreateUserResponse>(response);
        await mediator.Publish(new UserCreatedNotification(result), cancellationToken).ConfigureAwait(false);

        return result;
    }
}
