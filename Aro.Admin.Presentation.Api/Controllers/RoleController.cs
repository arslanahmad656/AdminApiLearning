using Aro.Admin.Application.Services.LogManager;
using Aro.Admin.Application.Services.Role;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Presentation.Api.DTOs;
using Aro.Admin.Presentation.Api.Filters;
using Microsoft.AspNetCore.Mvc;
using static Aro.Admin.Domain.Shared.PermissionCodes;

namespace Aro.Admin.Presentation.Api.Controllers;

[ApiController]
[Route("api/role")]
public class RoleController(IRoleService roleService, ILogManager<RoleController> logger) : ControllerBase
{
    [HttpGet("/rolebyids")]
    [Permissions(GetUserRoles)]
    public async Task<IActionResult> GetRoles([FromQuery] IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting GetRoles operation with roleIds: {RoleIds}", string.Join(", ", roleIds));
        
        // TODO: Bad design. In order to keep the consistency, use a query handler to get the roles.
        var roles = await roleService.GetByIds(roleIds, cancellationToken).ConfigureAwait(false);
        
        logger.LogDebug("Completed GetRoles operation successfully");
        return Ok(roles);
    }
}
