using Aro.Booking.Application.Mediator.Room.Commands;
using Aro.Booking.Application.Mediator.Room.DTOs;
using Aro.Booking.Application.Mediator.Room.Queries;
using Aro.Booking.Presentation.Api.DTOs.Room;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Domain.Shared;
using Aro.Common.Presentation.Shared.Filters;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aro.Booking.Presentation.Api.Controllers;

[ApiController]
[Route("api/room")]
public class RoomController(
    IMediator mediator,
    ILogManager<RoomController> logger
    ) : ControllerBase
{
    [HttpPost("create")]
    [Permissions(PermissionCodes.CreateRoom)]
    public async Task<IActionResult> CreateRoom(
        [FromBody] CreateRoomModel model,
        CancellationToken cancellationToken
        )
    {
        logger.LogDebug("Starting CreateRoom operation for room: {RoomName}", model.RoomName);

        var images = model.RoomImages == null
            ? []
            : model.RoomImages.Select(img =>
            {
                var bytes = Convert.FromBase64String(img.ContentBase64);
                var stream = new MemoryStream(bytes);

                return new RoomImageDto(
                    img.Name,
                    stream,
                    img.OrderIndex,
                    img.IsThumbnail
                );
            }).ToList();

        var response = await mediator.Send(new CreateRoomCommand(
            new(
                model.PropertyId,
                model.RoomName,
                model.RoomCode,
                model.Description,
                model.MaxOccupancy,
                model.MaxAdults,
                model.MaxChildren,
                model.RoomSizeSQM,
                (BedConfiguration)model.BedConfig,
                model.Amenities,
                images,
                model.IsActive
            )
        ), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed CreateRoom operation successfully with Id: {RoomId}",
            response.Id);

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
                query.PropertyId,
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

    [HttpPost("activate/{roomId:guid}")]
    [Permissions(PermissionCodes.PatchRoom)]
    public async Task<IActionResult> ActivateRoom(
        Guid roomId,
        CancellationToken cancellationToken
        )
    {
        logger.LogDebug("Starting ActivateRoom for Id: {RoomId}", roomId);

        var response = await mediator.Send(new ActivateRoomCommand(
            new(
                roomId
            )
        ), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed ActivateRoom for Id: {RoomId}", roomId);

        return Ok(response);
    }

    [HttpPost("deactivate/{roomId:guid}")]
    [Permissions(PermissionCodes.PatchRoom)]
    public async Task<IActionResult> DeactivateRoom(
        Guid roomId,
        CancellationToken cancellationToken
        )
    {
        logger.LogDebug("Starting DeactivateRoom for Id: {RoomId}", roomId);

        var response = await mediator.Send(new DeactivateRoomCommand(
            new(
                roomId
            )
        ), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed DeactivateRoom for Id: {RoomId}", roomId);

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

    [HttpGet("image/{roomId:guid}/{imageId:guid}")]
    public async Task<IActionResult> GetRoomImage(
        Guid roomId,
        Guid imageId,
        CancellationToken cancellationToken
        )
    {
        logger.LogDebug("Starting GetRoomImage for RoomId: {RoomId}, ImageId: {ImageId}", roomId, imageId);

        var response = await mediator.Send(new GetRoomImageQuery(roomId, imageId), cancellationToken).ConfigureAwait(false);

        string contentType = "image/jpeg";
        return File(response.Image, contentType);
    }

    [HttpPost("reorder")]
    [Permissions(PermissionCodes.PatchRoom)]
    public async Task<IActionResult> ReorderRooms(
        [FromBody] ReorderRoomsModel model,
        CancellationToken cancellationToken
        )
    {
        logger.LogDebug("Starting ReorderRooms for PropertyId: {PropertyId}", model.PropertyId);

        var roomOrders = model.RoomOrders.Select(ro =>
            new Application.Mediator.Room.DTOs.RoomOrderItemDto(ro.RoomId, ro.DisplayOrder)
        ).ToList();

        var response = await mediator.Send(new ReorderRoomsCommand(
            new(model.PropertyId, roomOrders)
        ), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed ReorderRooms for PropertyId: {PropertyId}", model.PropertyId);

        return Ok(response);
    }
}
