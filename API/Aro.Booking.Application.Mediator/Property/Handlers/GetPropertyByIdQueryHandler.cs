using Aro.Booking.Application.Mediator.Property.DTOs;
using Aro.Booking.Application.Mediator.Property.Queries;
using Aro.Booking.Application.Services.Property;
using Aro.Common.Application.Services.LogManager;
using MediatR;

namespace Aro.Booking.Application.Mediator.Property.Handlers;

public class GetPropertyByIdQueryHandler(IPropertyService propertyService, ILogManager<GetPropertyByIdQueryHandler> logger) : IRequestHandler<GetPropertyByIdQuery, DTOs.GetPropertyResponse>
{
    public async Task<DTOs.GetPropertyResponse> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Handling GetPropertiesByGroupIdQuery for PropertyId: {PropertyId}", request.Request.PropertyId);

        var serviceResponse = await propertyService.GetPropertyById(request.Request.PropertyId, cancellationToken).ConfigureAwait(false);

        return new(
            serviceResponse.PropertyId,
            serviceResponse.GroupId,
            serviceResponse.PropertyName,
            serviceResponse.PropertyTypes,
            serviceResponse.StarRating,
            serviceResponse.Currency,
            serviceResponse.Description,
            serviceResponse.AddressLine1,
            serviceResponse.AddressLine2,
            serviceResponse.City,
            serviceResponse.Country,
            serviceResponse.PostalCode,
            serviceResponse.PhoneNumber,
            serviceResponse.Website,
            serviceResponse.ContactName,
            serviceResponse.ContactEmail,
            serviceResponse.KeySellingPoints,
            serviceResponse.MarketingTitle,
            serviceResponse.MarketingDescription,
            serviceResponse.FileIds);
    }
}
