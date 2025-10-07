using Aro.Admin.Application.Services;
using Microsoft.AspNetCore.Http;

namespace Aro.Admin.Infrastructure.Services;

public class HttpContextBasedCurrentUserService(IHttpContextAccessor contextAccessor) : ICurrentUserService
{
    private readonly HttpContext? httpContext = contextAccessor.HttpContext;
    
    public Guid? GetCurrentUserId() => Guid.TryParse(httpContext?.User?.Identity?.Name, out var userId) ? userId : null;

    public bool IsAuthenticated() => httpContext?.User?.Identity?.IsAuthenticated ?? false;
}
