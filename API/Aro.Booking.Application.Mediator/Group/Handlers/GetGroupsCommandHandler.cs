using Aro.Booking.Application.Mediator.Group.Queries;
using Aro.Booking.Application.Services.Group;
using MediatR;

namespace Aro.Booking.Application.Mediator.Group.Handlers;

public class GetGroupsCommandHandler(IGroupService groupService) : IRequestHandler<GetGroupsQuery, DTOs.GetGroupsResponse>
{
    public async Task<DTOs.GetGroupsResponse> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
    {
        var req = request.Data;
        var res = await groupService.GetGroups(
            new(
                req.Filter,
                req.Include ?? string.Empty,
                req.Page,
                req.PageSize,
                req.SortBy,
                req.Ascending
                ), cancellationToken).ConfigureAwait(false);

        var groupDtos = res.Groups?
            .Select(g => new DTOs.GroupDto(
                g.Id,
                g.GroupName,
                g.AddressLine1,
                g.AddressLine2,
                g.City,
                g.PostalCode,
                g.Country,
                g.Logo,
                g.PrimaryContactId,
                g.PrimaryContactName,
                g.PrimaryContactEmail,
                g.IsActive
            ))
            .ToList() ?? [];

        var result = new DTOs.GetGroupsResponse(groupDtos, res.TotalCount);

        return result;
    }
}
