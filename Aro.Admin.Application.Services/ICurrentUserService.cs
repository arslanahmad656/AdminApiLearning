namespace Aro.Admin.Application.Services;

public interface ICurrentUserService
{
    Guid? GetCurrentUserId();

    bool IsAuthenticated();
}
