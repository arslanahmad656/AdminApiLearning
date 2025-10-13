using Aro.Admin.Application.Mediator.User.DTOs;
using Aro.Admin.Application.Mediator.User.Queries;
using Aro.Admin.Application.Services.DataServices;
using MediatR;

namespace Aro.Admin.Application.Mediator.User.Handlers;

public class GetBootstrapUserQueryHandler(IUserService userService) : IRequestHandler<GetBootstrapUserQuery, GetBootstrapUserResponse>
{
    public async Task<GetBootstrapUserResponse> Handle(GetBootstrapUserQuery request, CancellationToken cancellationToken)
    {
        var systemUser = await userService.GetSystemUser(request.BootstrapPassword, cancellationToken).ConfigureAwait(false);

        return new GetBootstrapUserResponse
        {
            DisplayName = systemUser.DisplayName,
            Email = systemUser.Email,
            Id = systemUser.Id,
        };
    }
}
