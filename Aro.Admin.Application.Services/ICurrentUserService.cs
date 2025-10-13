using Aro.Admin.Application.Services.DTOs.ServiceResponses;

namespace Aro.Admin.Application.Services;

public interface ICurrentUserService : IService
{
    Guid? GetCurrentUserId();

    bool IsAuthenticated();

    TokenInfo GetTokenInfo();
}
