using Aro.Admin.Presentation.UI.Models;

namespace Aro.Admin.Presentation.UI.Extensions;

public static class ModelDiffExtensions
{
    public static PatchGroupRequest? ToPatchGroupRequest(this GroupModel current, GroupModel original)
    {
        var patch = new PatchGroupRequest(
            Id: (Guid)current.Id!,
            GroupName: current.GroupName != original.GroupName ? current.GroupName : null,
            AddressLine1: current.AddressLine1 != original.AddressLine1 ? current.AddressLine1 : null,
            AddressLine2: current.AddressLine2 != original.AddressLine2 ? current.AddressLine2 : null,
            City: current.City != original.City ? current.City : null,
            Country: current.Country != original.Country ? current.Country : null,
            PostalCode: current.PostalCode != original.PostalCode ? current.PostalCode : null,
            PrimaryContactId: current.PrimaryContactId != original.PrimaryContactId
                ? current.PrimaryContactId
                : null
        );

        bool hasChanges =
            patch.GroupName != null ||
            patch.AddressLine1 != null ||
            patch.AddressLine2 != null ||
            patch.City != null ||
            patch.Country != null ||
            patch.PostalCode != null ||
            patch.PrimaryContactId != null;

        return hasChanges ? patch : null;
    }
}

