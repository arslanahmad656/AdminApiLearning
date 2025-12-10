using Aro.Booking.Application.Mediator.Policy.Commands;
using Aro.Booking.Application.Mediator.Policy.Queries;
using Aro.Booking.Presentation.Api.DTOs;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Domain.Shared;
using Aro.Common.Presentation.Shared.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Aro.Booking.Presentation.Api.Controllers;

[ApiController]
[Route("api/policy")]
public class PolicyController(
    IMediator mediator,
    ILogManager<PolicyController> logger
    ) : ControllerBase
{
    [HttpPost("create")]
    [Permissions(PermissionCodes.CreatePolicy)]
    public async Task<IActionResult> CreatePolicy(
        [FromBody] CreatePolicyModel model,
        CancellationToken cancellationToken
        )
    {
        logger.LogDebug("Starting CreatePolicy for Title: {Title}", model.Title);

        var response = await mediator.Send(new CreatePolicyCommand(
            new(
                model.Title,
                model.Description,
                model.IsActive
            )
        ), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed CreatePolicy successfully with Id: {PolicyId}", response.Id);

        return Ok(response);
    }

    [HttpGet("getall")]
    [Permissions(PermissionCodes.GetPolicies)]
    public async Task<IActionResult> GetPolicies(
        [FromQuery] GetPoliciesQueryParameters query,
        CancellationToken cancellationToken
        )
    {
        logger.LogDebug("Starting GetPolicies");

        var response = await mediator.Send(new GetPoliciesQuery(
            new(
                query.Filter,
                query.Include,
                query.Page,
                query.PageSize,
                query.SortBy,
                query.Ascending
            )
        ), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed GetPolicies with {Count} items", response.Policies.Count);

        return Ok(response);
    }

    [HttpGet("get/{policyId:guid}")]
    [Permissions(PermissionCodes.GetPolicy)]
    public async Task<IActionResult> GetPolicy(
        Guid policyId,
        [FromQuery] GetPolicyQueryParameters model,
        CancellationToken cancellationToken
        )
    {
        logger.LogDebug("Starting GetPolicy for Id: {PolicyId}", policyId);

        var response = await mediator.Send(new GetPolicyQuery(
            new(
                policyId,
                model.Include
            )
        ), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed GetPolicy for Id: {PolicyId}", policyId);

        return Ok(response);
    }

    [HttpPatch("patch/{policyId:guid}")]
    [Permissions(PermissionCodes.PatchPolicy)]
    public async Task<IActionResult> PatchPolicy(
        Guid policyId,
        [FromBody] PatchPolicyModel model,
        CancellationToken cancellationToken
        )
    {
        logger.LogDebug("Starting PatchPolicy for Id: {PolicyId}", policyId);

        var response = await mediator.Send(new PatchPolicyCommand(
            new(
                new(
                    policyId,
                    model.Title,
                    model.Description,
                    model.IsActive
                )
            )
        ), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed PatchPolicy for Id: {PolicyId}", policyId);

        return Ok(response);
    }

    [HttpDelete("delete/{policyId:guid}")]
    [Permissions(PermissionCodes.DeletePolicy)]
    public async Task<IActionResult> DeletePolicy(
        Guid policyId,
        CancellationToken cancellationToken
        )
    {
        logger.LogDebug("Starting DeletePolicy for Id: {PolicyId}", policyId);

        var response = await mediator.Send(new DeletePolicyCommand(
            new(
                policyId
            )
        ), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed DeletePolicy for Id: {PolicyId}", policyId);

        return Ok(response);
    }
}

