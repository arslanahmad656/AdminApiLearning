using Aro.Admin.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Aro.Admin.Presentation.Api.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class PermissionsAttribute(params string[] permissions) : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            context.Result = new ChallengeResult();
            return;
        }

        var authorizationService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();

        var userHasPermissions = await authorizationService.CurrentUserHasPermissions(permissions, context.HttpContext.RequestAborted).ConfigureAwait(false);

        if (!userHasPermissions)
        {
            context.Result = new ForbidResult();
        }
    }
}
