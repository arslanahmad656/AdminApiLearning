using Aro.Booking.Application.Mediator.Common.DTOs;
using Aro.Booking.Application.Mediator.Property.Commands;
using Aro.Booking.Application.Mediator.Property.DTOs;
using Aro.Booking.Application.Mediator.Property.Queries;
using Aro.Booking.Presentation.Api.DTOs;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Domain.Shared;
using Aro.Common.Infrastructure.Shared.Extensions;
using Aro.Common.Presentation.Shared.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Aro.Booking.Presentation.Api.Controllers;

[ApiController]
[Route("api/property")]
public class PropertyController(
    IMediator mediator,
    ILogManager<PropertyController> logger
    ) : ControllerBase
{
    [HttpPost("create")]
    [Permissions(PermissionCodes.CreateProperty)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateProperty(
        [FromForm] CreatePropertyModel model,
        CancellationToken cancellationToken
        )
    {
        logger.LogDebug("Starting CreateProperty operation for property: {PropertyName}",
            model.PropertyName);

        // TODO: refactor the following if branches to a method
        var fileData = new List<CreatePropertyRequest.FileData>();
        if (model.Files.Favicon is not null)
        {
            var favicon = new MemoryStream();
            await model.Files.Favicon.CopyToAsync(favicon, cancellationToken).ConfigureAwait(false);
            favicon.Position = 0;
            fileData.Add(new(nameof(model.Files.Favicon), favicon));
        }

        if (model.Files.Banner1 is not null)
        {
            var banner1 = new MemoryStream();
            await model.Files.Banner1.CopyToAsync(banner1, cancellationToken).ConfigureAwait(false);
            banner1.Position = 0;
            fileData.Add(new(nameof(model.Files.Banner1), banner1));
        }

        if (model.Files.Banner2 is not null)
        {
            var banner2 = new MemoryStream();
            await model.Files.Banner2.CopyToAsync(banner2, cancellationToken).ConfigureAwait(false);
            banner2.Position = 0;
            fileData.Add(new(nameof(model.Files.Banner2), banner2));
        }
        
        var response = await mediator.Send(new CreatePropertyCommand(
            new(
                model.GroupId,
                model.PropertyName,
                model.PropertyTypes,
                model.StarRating,
                model.Currency,
                model.Description,
                model.SetAddressSameAsGroupAddress,
                model.AddressLine1,
                model.AddressLine2,
                model.City,
                model.Country,
                model.PostalCode,
                model.PhoneNumber,
                model.Website,
                model.SetContactSameAsPrimaryContact,
                model.ContactName,
                model.ContactEmail,
                model.KeySellingPoints,
                model.MarketingTitle,
                model.MarketingDescription,
                fileData
            )
        ), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed CreateProperty operation successfully with Id: {PropertyId}",
            response.PropertyId);

        return Ok(response);
    }

    [HttpPut("update")]
    [Permissions(PermissionCodes.PatchProperty)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateProperty(
        [FromForm] UpdatePropertyModel model,
        CancellationToken cancellationToken
        )
    {
        logger.LogDebug("Starting UpdateProperty operation for property: {PropertyId}",
            model.PropertyId);

        var fileData = new List<UpdatePropertyRequest.FileData>();
        if (model.Files?.Favicon is not null)
        {
            var favicon = new MemoryStream();
            await model.Files.Favicon.CopyToAsync(favicon, cancellationToken).ConfigureAwait(false);
            favicon.Position = 0;
            fileData.Add(new(nameof(model.Files.Favicon), favicon));
        }

        if (model.Files?.Banner1 is not null)
        {
            var banner1 = new MemoryStream();
            await model.Files.Banner1.CopyToAsync(banner1, cancellationToken).ConfigureAwait(false);
            banner1.Position = 0;
            fileData.Add(new(nameof(model.Files.Banner1), banner1));
        }

        if (model.Files?.Banner2 is not null)
        {
            var banner2 = new MemoryStream();
            await model.Files.Banner2.CopyToAsync(banner2, cancellationToken).ConfigureAwait(false);
            banner2.Position = 0;
            fileData.Add(new(nameof(model.Files.Banner2), banner2));
        }

        var response = await mediator.Send(new UpdatePropertyCommand(
            new(
                model.PropertyId,
                model.GroupId,
                model.PropertyName,
                model.PropertyTypes,
                model.StarRating,
                model.Currency,
                model.Description,
                model.AddressLine1,
                model.AddressLine2,
                model.City,
                model.Country,
                model.PostalCode,
                model.PhoneNumber,
                model.Website,
                model.ContactName,
                model.ContactEmail,
                model.KeySellingPoints,
                model.MarketingTitle,
                model.MarketingDescription,
                fileData.Count > 0 ? fileData : null
            )
        ), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed UpdateProperty operation successfully with Id: {PropertyId}",
            response.PropertyId);

        return Ok(response);
    }

    [HttpGet("getbyid/{groupId:Guid}/{propertyId:Guid}")]
    public async Task<IActionResult> GetProperty(Guid groupId, Guid propertyId)
    {
        logger.LogDebug("Starting GetProperty operation for PropertyId: {PropertyId} in GroupId: {GroupId}",
            propertyId, groupId);
        var response = await mediator.Send(new GetPropertyByGroupAndPropertyIdQuery(
            new(
                groupId,
                propertyId
            )
        )).ConfigureAwait(false);

        logger.LogDebug("Completed GetProperty operation successfully with Id: {PropertyId}",
            response.PropertyId);

        return Ok(response);
    }

    [HttpGet("image/{groupId:Guid}/{propertyId:Guid}/{imageId:Guid}")]
    public async Task<IActionResult> GetImage(Guid groupId, Guid propertyId, Guid imageId)
    {
        logger.LogDebug("Starting GetImage operation for ImageId: {ImageId} in PropertyId: {PropertyId}, GroupId: {GroupId}",
            imageId, propertyId, groupId);

        var response = await mediator.Send(new GetPropertyImageQuery(
            new GetGroupEntityImageRequest(groupId, propertyId, imageId))).ConfigureAwait(false);

        string contentType = "image/jpeg";
        return File(response.Image, contentType);
    }

    [HttpGet("bygroup/{groupId:Guid}")]
    public async Task<IActionResult> GetPropertiesByGroupId(Guid groupId)
    {
        logger.LogDebug("Starting GetPropertiesByGroupId operation for GroupId: {GroupId}", groupId);

        var response = await mediator.Send(new GetPropertiesByGroupIdQuery(
            new GetPropertiesByGroupIdRequest(groupId))).ConfigureAwait(false);

        logger.LogDebug("Completed GetPropertiesByGroupId operation, found {Count} properties", response.Count);

        return Ok(response);
    }
}
