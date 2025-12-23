using Aro.UI.Application.DTOs.Group;
using System.Reflection;

namespace Aro.Admin.Presentation.UI.Extensions;

public static class PatchEntityRequestExtensions
{
    public static bool AllGroupPropertiesNull(this PatchGroupRequest patch)
    {
        if (patch == null)
        {
            return true;
        }

        // Exclude Id property from null check
        var properties = typeof(PatchGroupRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            if (prop.Name == nameof(PatchGroupRequest.Id))
                continue;

            var value = prop.GetValue(patch);
            if (value != null)
                return false;
        }
        return true;
    }
}

