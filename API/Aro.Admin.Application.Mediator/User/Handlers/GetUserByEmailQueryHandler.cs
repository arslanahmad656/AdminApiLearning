using Aro.Admin.Application.Mediator.User.Queries;
using Aro.Admin.Application.Services.User;
using MediatR;

namespace Aro.Admin.Application.Mediator.User.Handlers;

public class GetUserByEmailQueryHandler(IUserService userService) : IRequestHandler<GetUserByEmailQuery, DTOs.GetUserResponse>
{
    public async Task<DTOs.GetUserResponse> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var req = request.Data;
        var res = await userService.GetUserByEmail(req.Email, false, false, cancellationToken).ConfigureAwait(false);
        var user = res.User;

        return new DTOs.GetUserResponse(new(
            user.Id,
            user.Email,
            user.IsActive,
            user.DisplayName,
            user.PasswordHash,
            user.Roles.Select(r => new DTOs.GetRoleResponse(r.Id, r.Name, r.Description, r.IsBuiltin)),
            new DTOs.ContactInfo(user.ContactInfo.CountryCode, user.ContactInfo.PhoneNumber)
        ));
    }
}