namespace Aro.Common.Application.Services.RequestInterpretor;

public interface IRequestInterpretorService : IService
{
    Guid? GetCurrentUserId();

    bool IsAuthenticated();

    TokenInfo GetTokenInfo();

    string? RetrieveIpAddress();

    string? ExtractUsername();

    string? GetUserAgent();
}
