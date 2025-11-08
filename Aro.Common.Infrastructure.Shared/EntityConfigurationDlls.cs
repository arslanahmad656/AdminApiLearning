using System.Collections.ObjectModel;

namespace Aro.Common.Infrastructure.Shared;

public class EntityConfigurationDlls
{
    public readonly ReadOnlyCollection<string> Dlls = new(new[]
    {
        "Aro.Common.Infrastructure.Repository.dll",
        "Aro.Booking.Infrastructure.Repository.dll",
        "Aro.Admin.Infrastructure.Repository.dll",
    });
}
