using Aro.Admin.Application.Mediator.User.DTOs;
using Aro.Admin.Application.Mediator.User.Queries;
using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.SystemContext;
using Aro.Admin.Application.Services.User;
using MediatR;

namespace Aro.Admin.Application.Mediator.User.Handlers;

public class GetBootstrapUserQueryHandler(IUserService userService, ICurrentUserService currentUserService, ISystemContextFactory systemContextFactory) : IRequestHandler<GetBootstrapUserQuery, GetBootstrapUserResponse>
{
    public async Task<GetBootstrapUserResponse> Handle(GetBootstrapUserQuery request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated())
        {
            using var systemContext = systemContextFactory.Create();
            return await HandleInternal(request, cancellationToken).ConfigureAwait(false);
        }

        return await HandleInternal(request, cancellationToken).ConfigureAwait(false);
    }

    private async Task<GetBootstrapUserResponse> HandleInternal(GetBootstrapUserQuery request, CancellationToken cancellationToken)
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
