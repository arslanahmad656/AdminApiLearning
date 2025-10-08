using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Presentation.Api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Aro.Admin.Presentation.Api.Controllers;

[ApiController]
[Route("api/role")]
public class RoleController(IRoleService roleService) : ControllerBase
{
    [HttpGet("/rolebyids")]
    public async Task<IActionResult> GetRoles([FromQuery] IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default)
    {
        var roles = await roleService.GetByIds(roleIds, cancellationToken).ConfigureAwait(false);
        return Ok(roles);
    }
}
