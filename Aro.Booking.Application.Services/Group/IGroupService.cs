using Aro.Common.Application.Services;

namespace Aro.Booking.Application.Services.Group;

public interface IGroupService : IService
{
    Task<CreateGroupResponse> CreateGroup(CreateGroupDto group, CancellationToken cancellationToken = default);

    Task<GetGroupsResponse> GetGroups(GetGroupsDto query, CancellationToken cancellationToken = default);

    Task<GetGroupResponse> GetGroupById(GetGroupDto query, CancellationToken cancellationToken = default);

    Task<PatchGroupResponse> PatchGroup(PatchGroupDto group, CancellationToken cancellationToken = default);

    Task<DeleteGroupResponse> DeleteGroup(DeleteGroupDto group, CancellationToken cancellationToken = default);

}
