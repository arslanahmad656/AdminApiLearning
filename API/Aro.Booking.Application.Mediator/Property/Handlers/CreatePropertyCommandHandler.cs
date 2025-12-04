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
                request.Request.Description,
                request.Request.SetAddressSameAsGroupAddress,
                request.Request.AddressLine1,
                request.Request.AddressLine2,
                request.Request.City,
                request.Request.Country,
                request.Request.PostalCode,
                request.Request.PhoneNumber,
                request.Request.Website,
                request.Request.SetContactSameAsGroupContact,
                request.Request.ContactName,
                request.Request.ContactEmail,
                request.Request.KeySellingPoints,
                request.Request.MarketingTitle,
                request.Request.MarketingDescription,
                request.Request.Files.Select(f => new CreatePropertyDto.FileData(f.FileName, f.Content)).ToList()
            ),
            cancellationToken
        ).ConfigureAwait(false);

        await mediator.Publish(
            new PropertyCreatedNotification(
                serviceResponse.PropertyId,
                serviceResponse.GroupId,
                serviceResponse.PropertyName
            ),
            cancellationToken
        ).ConfigureAwait(false);

        return new DTOs.CreatePropertyResponse(
            serviceResponse.PropertyId,
            serviceResponse.GroupId
        );
    }
}
