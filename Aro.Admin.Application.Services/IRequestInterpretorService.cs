namespace Aro.Admin.Application.Services;

public interface IRequestInterpretorService
{
    string? RetrieveIpAddress();

    string? ExtractUsername();
}
