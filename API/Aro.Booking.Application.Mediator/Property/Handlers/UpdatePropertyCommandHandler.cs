using Aro.Booking.Application.Mediator.Property.Commands;
using Aro.Booking.Application.Services.Property;
using MediatR;

namespace Aro.Booking.Application.Mediator.Property.Handlers;

public class UpdatePropertyCommandHandler(IPropertyService propertyService)
    : IRequestHandler<UpdatePropertyCommand, DTOs.UpdatePropertyResponse>
{
    public async Task<DTOs.UpdatePropertyResponse> Handle(
        UpdatePropertyCommand request,
        CancellationToken cancellationToken)
    {
        var serviceResponse = await propertyService.UpdateProperty(
            new UpdatePropertyDto(
                request.Request.PropertyId,
                request.Request.GroupId,
                request.Request.PropertyName,
                request.Request.PropertyTypes,
                request.Request.StarRating,
                request.Request.Currency,
                request.Request.Description,
                request.Request.AddressLine1,
                request.Request.AddressLine2,
                request.Request.City,
                request.Request.Country,
                request.Request.PostalCode,
                request.Request.PhoneNumber,
                request.Request.Website,
                request.Request.ContactName,
                request.Request.ContactEmail,
                request.Request.KeySellingPoints,
                request.Request.MarketingTitle,
                request.Request.MarketingDescription,
                request.Request.Files?.Select(f => new UpdatePropertyDto.FileData(f.FileName, f.Content)).ToList(),
                request.Request.SetContactSameAsGroupContact,
                request.Request.SetAddressSameAsGroupAddress
            ),
            cancellationToken
        ).ConfigureAwait(false);

        return new DTOs.UpdatePropertyResponse(
            serviceResponse.PropertyId,
            serviceResponse.GroupId
        );
    }
}
