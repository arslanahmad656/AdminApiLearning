using Aro.Booking.Application.Mediator.Amenity.Commands;
using Aro.Booking.Application.Mediator.Amenity.Queries;
using Aro.Booking.Presentation.Api.DTOs;
using Aro.Common.Domain.Shared;
using Aro.Common.Presentation.Shared.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Aro.Booking.Presentation.Api.Controllers;

[ApiController]
[Route("api/amenity")]
public class AmenityController(
    IMediator mediator
    //ILogManager<AmenityController> logger
    ) : ControllerBase
{
    [HttpPost("create")]
    [Permissions(PermissionCodes.CreateAmenity)]
    public async Task<IActionResult> CreateAmenity(
        [FromBody] CreateAmenityModel model,
        CancellationToken cancellationToken
        )
    {
        var response = await mediator.Send(new CreateAmenityCommand(
            new(
                model.Name
            )
        ), cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpGet("getall")]
    [Permissions(PermissionCodes.GetAmenities)]
    public async Task<IActionResult> GetAmenitys(
        [FromQuery] GetAmenitiesQueryParameters query,
        CancellationToken cancellationToken
        )
    {
        var response = await mediator.Send(new GetAmenitiesQuery(
            new(
                query.Filter,
                query.Include,
                query.Page,
                query.PageSize,
                query.SortBy,
                query.Ascending
            )
        ), cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpGet("get/{amenityId:guid}")]
    [Permissions(PermissionCodes.GetAmenity)]
    public async Task<IActionResult> GetAmenity(
        Guid amenityId,
        [FromQuery] GetAmenityQueryParameters model,
        CancellationToken cancellationToken
        )
    {
        var response = await mediator.Send(new GetAmenityQuery(
            new(
                amenityId,
                model.Include
            )
        ), cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpPatch("patch/{amenityId:guid}")]
    [Permissions(PermissionCodes.PatchAmenity)]
    public async Task<IActionResult> PatchRoom(
    Guid amenityId,
    [FromBody] PatchAmenityModel model,
    CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(
            new PatchAmenityCommand(
                new( // PatchAmenityRequest
                    new( // AmenityPatchDto
                        amenityId,
                        model.Name
                    )
                )
        ), cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpDelete("delete/{amenityId:guid}")]
    [Permissions(PermissionCodes.DeleteAmenity)]
    public async Task<IActionResult> DeleteAmenity(
        Guid amenityId,
        CancellationToken cancellationToken
        )
    {
        var response = await mediator.Send(new DeleteAmenityCommand(
            new(
                amenityId
            )
        ), cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }
}
