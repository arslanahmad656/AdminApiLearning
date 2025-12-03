using Aro.Booking.Application.Mediator.Property.DTOs;
using Aro.Booking.Application.Mediator.Property.Queries;
using Aro.Booking.Application.Services.Property;
using Aro.Common.Application.Services.LogManager;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Aro.Booking.Application.Mediator.Property.Handlers;

public class GetPropertiesByGroupIdQueryHandler(IPropertyService propertyService, ILogManager<GetPropertiesByGroupIdQueryHandler> logger) : IRequestHandler<GetPropertiesByGroupIdQuery, List<DTOs.GetPropertyResponse>>
{
    public async Task<List<DTOs.GetPropertyResponse>> Handle(GetPropertiesByGroupIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Handling GetPropertiesByGroupIdQuery for GroupId: {GroupId}", request.Request.GroupId);

        var serviceResponse = await propertyService.GetPropertiesByGroupId(request.Request.GroupId, cancellationToken).ConfigureAwait(false);

        return serviceResponse.Select(property => new DTOs.GetPropertyResponse(
            property.PropertyId,
            property.GroupId,
            property.PropertyName,
            property.PropertyTypes,
            property.StarRating,
            property.Currency,
            property.Description,
            property.AddressLine1,
            property.AddressLine2,
            property.City,
            property.Country,
            property.PostalCode,
            property.PhoneNumber,
            property.Website,
            property.ContactName,
            property.ContactEmail,
            property.KeySellingPoints,
            property.MarketingTitle,
            property.MarketingDescription,
            property.FileIds
            )).ToList();
    }
}
