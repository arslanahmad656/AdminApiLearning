namespace Aro.Admin.Infrastructure.Services;

public partial class AuthorizationService
{
    private bool IsSystemContext()
    {
        if (systemContext.IsEnabled)
        {
            logger.LogWarn($"Skipping the permission validation since the code is running under the system context.");
            return true;
        }

        return false;
    }
}
