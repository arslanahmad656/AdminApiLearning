using Aro.Booking.Application.Mediator.Group.Queries;
using Aro.Booking.Application.Services.Group;
using MediatR;

namespace Aro.Booking.Application.Mediator.Group.Handlers;

public class GetGroupCommandHandler(IGroupService groupService) : IRequestHandler<GetGroupQuery, DTOs.GetGroupResponse>
{
    public async Task<DTOs.GetGroupResponse> Handle(GetGroupQuery request, CancellationToken cancellationToken)
    {
        var req = request.Data;
        var res = await groupService.GetGroupById(
            new(
                req.Id,
                req.Include ?? string.Empty
                ), cancellationToken).ConfigureAwait(false);

        var g = res.Group;
        var groupDto = new DTOs.GroupDto(
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

        var result = new DTOs.GetGroupResponse(groupDto);

        return result;
    }
}
