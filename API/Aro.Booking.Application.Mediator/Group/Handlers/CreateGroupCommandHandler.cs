using Aro.Booking.Application.Mediator.Group.Commands;
using Aro.Booking.Application.Mediator.Group.Notifications;
using Aro.Booking.Application.Services.Group;
using MediatR;

namespace Aro.Booking.Application.Mediator.Group.Handlers;

public class CreateGroupCommandHandler(IGroupService groupService, IMediator mediator) : IRequestHandler<CreateGroupCommand, DTOs.CreateGroupResponse>
{
    public async Task<DTOs.CreateGroupResponse> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var r = request.CreateGroupRequest;

        var logo = new GroupLogo(r.Logo.Name, r.Logo.Content);

        var primaryContact = new PrimaryContact(
            r.PrimaryContact.Email,
            r.PrimaryContact.Name,
            r.PrimaryContact.CountryCode,
            r.PrimaryContact.PhoneNumber
        );

        var response = await groupService.CreateGroup(
            new CreateGroupDto(
                r.GroupName,
                r.AddressLine1,
                r.AddressLine2,
                r.City,
                r.Country,
                r.PostalCode,
                logo,
                primaryContact,
                r.IsActive
            ), cancellationToken
        ).ConfigureAwait(false);

        var result = new DTOs.CreateGroupResponse(response.Id, response.GroupName);
        await mediator.Publish(new GroupCreatedNotification(result), cancellationToken).ConfigureAwait(false);

        return result;
    }
}
