namespace Aro.Admin.Application.Services;

public interface IRequestInterpretorService : IService
{
    string? RetrieveIpAddress();

    string? ExtractUsername();

    string? GetUserAgent();
}
