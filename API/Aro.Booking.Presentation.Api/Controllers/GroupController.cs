using Aro.Booking.Application.Mediator.Common.DTOs;
using Aro.Booking.Application.Mediator.Group.Commands;
using Aro.Booking.Application.Mediator.Group.Queries;
using Aro.Booking.Application.Mediator.Property.Queries;
using Aro.Booking.Presentation.Api.DTOs;
using Aro.Common.Domain.Shared;
using Aro.Common.Presentation.Shared.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Aro.Booking.Presentation.Api.Controllers;

[ApiController]
[Route("api/group")]
public class GroupController(
    IMediator mediator
    //ILogManager<GroupController> logger
    ) : ControllerBase
{
    [HttpPost("create")]
    [Permissions(PermissionCodes.CreateGroup)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateGroup(
        [FromForm] CreateGroupModel model,
        CancellationToken cancellationToken
        )
    {
        //logger.LogDebug("Starting CreateGroup operation for group: {0}",
        //    model.GroupName);

        var logo = new MemoryStream();
        await model.Logo.CopyToAsync(logo, cancellationToken).ConfigureAwait(false);
        logo.Position = 0;

        var response = await mediator.Send(new CreateGroupCommand(
            new(
                model.GroupName,
                model.AddressLine1,
                model.AddressLine2,
                model.City,
                model.PostalCode,
                model.Country,
                logo,
                model.PrimaryContactId,
                model.IsActive
            )
        ), cancellationToken).ConfigureAwait(false);

        //logger.LogDebug("Completed CreateGroup operation successfully");

        return Ok(response);
    }

    [HttpGet("getall")]
    [Permissions(PermissionCodes.GetGroups)]
    public async Task<IActionResult> GetGroups(
        [FromQuery] GetGroupsQueryParameters query,
        CancellationToken cancellationToken
        )
    {
        //logger.LogDebug("Starting GetGroups operation");

        var response = await mediator.Send(new GetGroupsQuery(
            new(
                query.Filter,
                query.Include,
                query.Page,
                query.PageSize,
                query.SortBy,
                query.Ascending
            )
        ), cancellationToken).ConfigureAwait(false);

        //logger.LogDebug("Completed GetGroups operation successfully");

        return Ok(response);
    }

    [HttpGet("get/{groupId:guid}")]
    [Permissions(PermissionCodes.GetGroup)]
    public async Task<IActionResult> GetGroup(
        Guid groupId,
        [FromQuery] GetGroupQueryParameters model,
        CancellationToken cancellationToken
        )
    {
        //logger.LogDebug("Starting GetGroup operation for group: {0}",
        //    groupId);

        var response = await mediator.Send(new GetGroupQuery(
            new(
                groupId,
                model.Include
            )
        ), cancellationToken).ConfigureAwait(false);

        //logger.LogDebug("Completed GetGroup operation successfully");

        return Ok(response);
    }

    [HttpPatch("patch/{groupId:guid}")]
    [Permissions(PermissionCodes.PatchGroup)]
    public async Task<IActionResult> PatchGroup(
        Guid groupId,
        [FromBody] PatchGroupModel model,
        CancellationToken cancellationToken
        )
    {
        //logger.LogDebug("Starting PatchGroup operation for group: {0}",
        //    groupId);

        var response = await mediator.Send(new PatchGroupCommand(
            new(
                groupId,
                model.GroupName,
                model.AddressLine1,
                model.AddressLine2,
                model.City,
                model.PostalCode,
                model.Country,
                model.Logo,
                model.PrimaryContactId,
                model.IsActive
            )
        ), cancellationToken).ConfigureAwait(false);

        //logger.LogDebug("Completed PatchGroup operation successfully");

        return Ok(response);
    }

    [HttpDelete("delete/{groupId:guid}")]
    [Permissions(PermissionCodes.DeleteGroup)]
    public async Task<IActionResult> DeleteGroup(
        Guid groupId,
        CancellationToken cancellationToken
        )
    {
        //logger.LogDebug("Starting DeleteGroup operation for group: {0}",
        //    groupId);

        var response = await mediator.Send(new DeleteGroupCommand(
            new(
                groupId
            )
        ), cancellationToken).ConfigureAwait(false);

        //logger.LogDebug("Completed DeleteGroup operation successfully");

        return Ok(response);
    }

    [HttpGet("image/{groupId:Guid}/{imageId:Guid}")]
    public async Task<IActionResult> GetImage(Guid groupId, Guid imageId)
    {
        var response = await mediator.Send(new GetPropertyImageQuery(
            new GetGroupEntityImageRequest(groupId, groupId, imageId))).ConfigureAwait(false);

        string contentType = "image/jpeg";
        return File(response.Image, contentType);
    }
}
