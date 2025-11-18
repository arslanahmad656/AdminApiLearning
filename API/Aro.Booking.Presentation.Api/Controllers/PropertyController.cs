using Aro.Booking.Application.Mediator.Property.Commands;
using Aro.Booking.Presentation.Api.DTOs;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Domain.Shared;
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
    public async Task<IActionResult> CreateProperty(
        [FromBody] CreatePropertyModel model,
        CancellationToken cancellationToken
        )
    {
        logger.LogDebug("Starting CreateProperty operation for property: {PropertyName}",
            model.PropertyName);

        var response = await mediator.Send(new CreatePropertyCommand(
            new(
                model.GroupId,
                model.PropertyName,
                model.PropertyTypes,
                model.StarRating,
                model.Currency,
                model.Description
            )
        ), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed CreateProperty operation successfully with Id: {PropertyId}",
            response.Id);

        return Ok(response);
    }
}
