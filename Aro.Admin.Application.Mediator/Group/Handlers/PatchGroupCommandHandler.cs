using Aro.Admin.Application.Mediator.Group.Commands;
using Aro.Admin.Application.Mediator.Group.DTOs;
using Aro.Admin.Application.Mediator.Group.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters;
using MediatR;

namespace Aro.Admin.Application.Mediator.Group.Handlers;

public class PatchGroupCommandHandler(IGroupService groupService, IMediator mediator) : IRequestHandler<PatchGroupCommand, PatchGroupResponse>
{
    public async Task<PatchGroupResponse> Handle(PatchGroupCommand request, CancellationToken cancellationToken)
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
                req.Logo,
                req.PrimaryContactId,
                req.IsActive
            ), cancellationToken
        ).ConfigureAwait(false);

        var result = new PatchGroupResponse(
                res.Id,
                res.GroupName,
                res.AddressLine1,
                res.AddressLine2,
                res.City,
                res.PostalCode,
                res.Country,
                res.Logo,
                res.PrimaryContactId,
                res.IsActive
            );

        await mediator.Publish(new GroupPatchedNotification(result), cancellationToken).ConfigureAwait(false);

        return result;
    }
}
