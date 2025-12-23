using Aro.Booking.Application.Mediator.Group.Commands;
using Aro.Booking.Application.Mediator.Group.Notifications;
using Aro.Booking.Application.Services.Group;
using MediatR;

namespace Aro.Booking.Application.Mediator.Group.Handlers;

public class PatchGroupCommandHandler(IGroupService groupService, IMediator mediator) : IRequestHandler<PatchGroupCommand, DTOs.PatchGroupResponse>
{
    public async Task<DTOs.PatchGroupResponse> Handle(PatchGroupCommand request, CancellationToken cancellationToken)
    {
        var req = request.PatchGroupRequest;
        var res = await groupService.PatchGroup(
            new(
                req.Id,
                req.GroupName,
                req.AddressLine1,
                req.AddressLine2,
                req.City,
                req.PostalCode,
                req.Country,
                req.Logo == null ? null : new(req.Logo.Name, req.Logo.Content),
                req.PrimaryContactId,
                req.IsActive
            ), cancellationToken
        ).ConfigureAwait(false);

        var result = new DTOs.PatchGroupResponse(
                res.Id,
                res.GroupName,
                res.AddressLine1,
                res.AddressLine2,
                res.City,
                res.PostalCode,
                res.Country,
                res.LogoId,
                res.PrimaryContactId,
                res.IsActive
            );

        await mediator.Publish(new GroupPatchedNotification(result), cancellationToken).ConfigureAwait(false);

        return result;
    }
}
