using Aro.Admin.Presentation.UI.Models;

namespace Aro.Admin.Presentation.UI.Services;

public interface IGroupService
{
    Task<CreateGroupResponse?> CreateGroup(CreateGroupRequest request);

    Task<GetGroupResponse?> GetGroup(GetGroupRequest request);

    Task<GetGroupsResponse?> GetGroups(GetGroupsRequest request);

    Task<PatchGroupResponse> PatchGroup(PatchGroupRequest request);

    Task<DeleteGroupResponse?> DeleteGroup(Guid Id);
}
