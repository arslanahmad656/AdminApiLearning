using Aro.Booking.Application.Mediator.Room.Commands;
using Aro.Booking.Application.Mediator.Room.Queries;
using Aro.Booking.Presentation.Api.DTOs;
using Aro.Common.Domain.Shared;
using Aro.Common.Presentation.Shared.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Aro.Booking.Presentation.Api.Controllers;

[ApiController]
[Route("api/room")]
public class RoomController(
    IMediator mediator
    //ILogManager<RoomController> logger
    ) : ControllerBase
{
    [HttpPost("create")]
    [Permissions(PermissionCodes.CreateRoom)]
    public async Task<IActionResult> CreateRoom(
        [FromBody] CreateRoomModel model,
        CancellationToken cancellationToken
        )
    {
        var response = await mediator.Send(new CreateRoomCommand(
            new(
                model.RoomName,
                model.RoomCode,
                model.Description,
                model.MaxOccupancy,
                model.MaxAdults,
                model.MaxChildren,
                model.RoomSizeSQM,
                model.RoomSizeSQFT,
                (Aro.Booking.Application.Mediator.Room.DTOs.BedConfiguration)model.BedConfig,
                model.AmenityIds,
                model.IsActive
            )
        ), cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpGet("getall")]
    [Permissions(PermissionCodes.GetRooms)]
    public async Task<IActionResult> GetRooms(
        [FromQuery] GetRoomsQueryParameters query,
        CancellationToken cancellationToken
        )
    {
        var response = await mediator.Send(new GetRoomsQuery(
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

    [HttpGet("get/{roomId:guid}")]
    [Permissions(PermissionCodes.GetRoom)]
    public async Task<IActionResult> GetRoom(
        Guid roomId,
        [FromQuery] GetRoomQueryParameters model,
        CancellationToken cancellationToken
        )
    {
        var response = await mediator.Send(new GetRoomQuery(
            new(
                roomId,
                model.Include
            )
        ), cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpPatch("patch/{roomId:guid}")]
    [Permissions(PermissionCodes.PatchRoom)]
    public async Task<IActionResult> PatchRoom(
        Guid roomId,
        [FromBody] PatchRoomModel model,
        CancellationToken cancellationToken
        )
    {
        var response = await mediator.Send(
            new PatchRoomCommand(
                new( // PatchRoomRequest
                    new( // RoomPatchDto
                    roomId,
                    model.RoomName,
                    model.RoomCode,
                    model.Description,
                    model.MaxOccupancy,
                    model.MaxAdults,
                    model.MaxChildren,
                    model.RoomSizeSQM,
                    model.RoomSizeSQFT,
                    (Application.Mediator.Room.DTOs.BedConfiguration?)model.BedConfig,
                    model.AmenityIds,
                    model.IsActive
                    )
                )
        ), cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpDelete("delete/{roomId:guid}")]
    [Permissions(PermissionCodes.DeleteRoom)]
    public async Task<IActionResult> DeleteRoom(
        Guid roomId,
        CancellationToken cancellationToken
        )
    {
        var response = await mediator.Send(new DeleteRoomCommand(
            new(
                roomId
            )
        ), cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }
}
