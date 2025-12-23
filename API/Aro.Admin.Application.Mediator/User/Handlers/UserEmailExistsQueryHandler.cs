using Aro.Admin.Application.Mediator.User.Queries;
using Aro.Admin.Application.Services.User;
using MediatR;

namespace Aro.Admin.Application.Mediator.User.Handlers;

public class UserEmailExistsQueryHandler(IUserService userService) : IRequestHandler<UserEmailExistsQuery, bool>
{
    public async Task<bool> Handle(UserEmailExistsQuery request, CancellationToken cancellationToken)
    {
        var req = request.Data;
        var res = await userService.UserEmailExists(req.Email, cancellationToken).ConfigureAwait(false);

        return res;
    }
}