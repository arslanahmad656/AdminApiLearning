using Aro.Booking.Application.Mediator.Property.Commands;
using Aro.Booking.Application.Mediator.Property.Notifications;
using Aro.Booking.Application.Services.Property;
using MediatR;

namespace Aro.Booking.Application.Mediator.Property.Handlers;

public class CreatePropertyCommandHandler(IPropertyService propertyService, IMediator mediator)
    : IRequestHandler<CreatePropertyCommand, DTOs.CreatePropertyResponse>
{
    public async Task<DTOs.CreatePropertyResponse> Handle(
        CreatePropertyCommand request,
        CancellationToken cancellationToken)
    {
        var serviceResponse = await propertyService.CreateProperty(
            new CreatePropertyDto(
                request.Request.GroupId,
                request.Request.PropertyName,
                request.Request.PropertyTypes,
                request.Request.StarRating,
                request.Request.Currency,
                request.Request.Description
            ),
            cancellationToken
        ).ConfigureAwait(false);

        await mediator.Publish(
            new PropertyCreatedNotification(
                serviceResponse.Id,
                serviceResponse.GroupId,
                serviceResponse.PropertyName,
                serviceResponse.PropertyTypes,
                serviceResponse.StarRating,
                serviceResponse.Currency
            ),
            cancellationToken
        ).ConfigureAwait(false);

        return new DTOs.CreatePropertyResponse(
            serviceResponse.Id,
            serviceResponse.GroupId,
            serviceResponse.PropertyName,
            serviceResponse.PropertyTypes,
            serviceResponse.StarRating,
            serviceResponse.Currency,
            serviceResponse.Description
        );
    }
}
