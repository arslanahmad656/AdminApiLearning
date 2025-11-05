using Aro.Admin.Application.Mediator.Group.Commands;
using Aro.Admin.Application.Mediator.Group.DTOs;
using Aro.Admin.Application.Mediator.Group.Notifications;
using Aro.Admin.Application.Mediator.Group.Queries;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters;
using MediatR;

namespace Aro.Admin.Application.Mediator.Group.Handlers;

public class GetGroupsCommandHandler(IGroupService groupService) : IRequestHandler<GetGroupsQuery, GetGroupsResponse>
{
    public async Task<GetGroupsResponse> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
    {
        var req = request.Data;
        var res = await groupService.GetGroups(
            new(
                req.Include,
                req.Page,
                req.PageSize,
                req.SortBy,
                req.Ascending
                ), cancellationToken).ConfigureAwait(false);

        var groupDtos = res.Groups?
            .Select(g => new GroupDto(
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
            ))
            .ToList() ?? [];

        var result = new GetGroupsResponse(groupDtos);

        return result;
    }
}
