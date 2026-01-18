using Aro.Common.Application.Shared;

namespace Aro.Common.Application.Services.RequestInterpretor;

public interface IRequestInterpretorService : IService
{
    Guid? GetCurrentUserId();

    bool IsAuthenticated();

    TokenInfo GetTokenInfo();

    string? RetrieveIpAddress(bool stripPort = false);

    string? ExtractUsername();

    string? GetUserAgent();
}
