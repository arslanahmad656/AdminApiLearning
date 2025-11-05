using Aro.Admin.Application.Mediator.Group.Commands;
using Aro.Admin.Application.Mediator.Group.DTOs;
using Aro.Admin.Application.Mediator.Group.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters;
using MediatR;

namespace Aro.Admin.Application.Mediator.Group.Handlers;

public class CreateGroupCommandHandler(IGroupService groupService, IMediator mediator) : IRequestHandler<CreateGroupCommand, CreateGroupResponse>
{
    public async Task<CreateGroupResponse> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var r = request.CreateGroupRequest;
        var response = await groupService.CreateGroup(
            new(
                r.GroupName,
                r.AddressLine1,
                r.AddressLine2,
                r.City,
                r.PostalCode,
                r.Country,
                r.Logo,
                r.PrimaryContactId,
                r.IsActive
            ), cancellationToken
        ).ConfigureAwait(false);

        var result = new CreateGroupResponse(response.Id, response.GroupName);
        await mediator.Publish(new GroupCreatedNotification(result), cancellationToken).ConfigureAwait(false);

        return result;
    }
}
