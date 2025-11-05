using Aro.Admin.Application.Mediator.Group.Commands;
using Aro.Admin.Application.Mediator.Group.DTOs;
using Aro.Admin.Application.Mediator.Group.Notifications;
using Aro.Admin.Application.Mediator.Group.Queries;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters;
using MediatR;

namespace Aro.Admin.Application.Mediator.Group.Handlers;

public class GetGroupCommandHandler(IGroupService groupService) : IRequestHandler<GetGroupQuery, GetGroupResponse>
{
    public async Task<GetGroupResponse> Handle(GetGroupQuery request, CancellationToken cancellationToken)
    {
        var req = request.Data;
        var res = await groupService.GetGroupById(
            new(
                req.Id,
                req.Include
                ), cancellationToken).ConfigureAwait(false);

        var g = res.Group;
        var groupDto = new GroupDto(
                g.Id,
                g.GroupName,
                g.AddressLine1,
                g.AddressLine2,
                g.City,
                g.PostalCode,
                g.Country,
                g.Logo,
                g.PrimaryContactId,
                g.IsActive
            );

        var result = new GetGroupResponse(groupDto);

        return result;
    }
}
