using Aro.UI.Application.DTOs.Group;

namespace Aro.UI.Infrastructure.Services;

public interface IGroupService
{
    Task<CreateGroupResponse?> CreateGroup(CreateGroupRequest request);

    Task<GetGroupResponse?> GetGroup(GetGroupRequest request);

    Task<GetGroupsResponse?> GetGroups(GetGroupsRequest request);

    Task<PatchGroupResponse?> PatchGroup(PatchGroupRequest request);

    Task<DeleteGroupResponse?> DeleteGroup(Guid Id);
}
