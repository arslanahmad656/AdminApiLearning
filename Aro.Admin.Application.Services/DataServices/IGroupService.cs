using Aro.Admin.Application.Services.DTOs.ServiceParameters;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;

namespace Aro.Admin.Application.Services.DataServices;

public interface IGroupService : IService
{
    Task<CreateGroupResponse> CreateGroup(CreateGroupDto group, CancellationToken cancellationToken = default);

    Task<GetGroupsResponse> GetGroups(GetGroupsDto query, CancellationToken cancellationToken = default);

    Task<GetGroupResponse> GetGroupById(GetGroupDto query, CancellationToken cancellationToken = default);

    Task<PatchGroupResponse> PatchGroup(PatchGroupDto group, CancellationToken cancellationToken = default);

    Task<DeleteGroupResponse> DeleteGroup(DeleteGroupDto group, CancellationToken cancellationToken = default);

}
