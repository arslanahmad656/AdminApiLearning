using Aro.Admin.Application.Mediator.User.DTOs;
using Aro.Admin.Application.Mediator.User.Queries;
using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using MediatR;

namespace Aro.Admin.Application.Mediator.User.Handlers;

public class GetBootstrapUserQueryHandler(IUserService userService, ICurrentUserService currentUserService, ISystemContext systemContext) : IRequestHandler<GetBootstrapUserQuery, GetBootstrapUserResponse>
{
    public async Task<GetBootstrapUserResponse> Handle(GetBootstrapUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (!currentUserService.IsAuthenticated())
            {
                systemContext.IsSystemContext = true;
            }

            var systemUser = await userService.GetSystemUser(request.BootstrapPassword, cancellationToken).ConfigureAwait(false);

            return new GetBootstrapUserResponse
            {
                DisplayName = systemUser.DisplayName,
                Email = systemUser.Email,
                Id = systemUser.Id,
            };
        }
        finally
        {
            systemContext.IsSystemContext = false;
        }
    }
}
