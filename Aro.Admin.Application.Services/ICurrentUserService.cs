using Aro.Admin.Application.Services.DTOs.ServiceResponses;

namespace Aro.Admin.Application.Services;

public interface ICurrentUserService
{
    Guid? GetCurrentUserId();

    bool IsAuthenticated();

    TokenInfo GetTokenInfo();
}
