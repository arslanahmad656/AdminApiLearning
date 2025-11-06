using Aro.Booking.Application.Services.Group;

namespace Aro.Booking.Infrastructure.Services;

public partial class GroupService(
    IRepositoryManager repository,
    IUniqueIdGenerator idGenerator,
    IAuthorizationService authorizationService,
    ILogManager<GroupService> logger,
    ErrorCodes errorCodes
) : IGroupService
{
    private readonly IGroupRepository groupRepository = repository.GroupRepository;
    private readonly IUserRepository userRepository = repository.UserRepository;

    public async Task<CreateGroupResponse> CreateGroup(CreateGroupDto group, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.CreateGroup], cancellationToken);

        var query = userRepository.GetById(group.PrimaryContactId);
        _ = await query
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false) ??
            throw new AroUserNotFoundException(group.PrimaryContactId.ToString());

        var GroupEntity = new Group
        {
            Id = idGenerator.Generate(),
            GroupName = group.GroupName,
            AddressLine1 = group.AddressLine1,
            AddressLine2 = group.AddressLine2,
            City = group.City,
            Country = group.Country,
            PostalCode = group.PostalCode,
            Logo = group.Logo,
            PrimaryContactId = group.PrimaryContactId,
            IsActive = group.IsActive
        };

        await groupRepository.Create(GroupEntity, cancellationToken).ConfigureAwait(false);

        await repository.SaveChanges(cancellationToken).ConfigureAwait(false);

        return new CreateGroupResponse(GroupEntity.Id, GroupEntity.GroupName);
    }

    public async Task<GetGroupsResponse> GetGroups(
        GetGroupsDto query,
        CancellationToken cancellationToken = default
        )
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.GetGroups], cancellationToken);

        var groups = groupRepository.GetAll()
            .IncludeElements(query.Include)
            .SortBy(query.SortBy, query.Ascending)
            .Paginate(query.Page, query.PageSize);


        var groupDtos = await groups
            .Select(g => new GroupDto(
                g.Id,
                g.GroupName,
                g.AddressLine1,
                g.AddressLine2,
                g.City,
                g.PostalCode,
                g.Country,
                g.Logo,
                g.PrimaryContactId,
                g.IsActive
            ))
            .ToListAsync(cancellationToken);

        return new GetGroupsResponse(groupDtos);
    }

    public async Task<GetGroupResponse> GetGroupById(
        GetGroupDto dto,
        CancellationToken cancellationToken = default
        )
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.GetGroup], cancellationToken);

        var query = groupRepository.GetById(dto.Id);

        var response = await query
            .IncludeElements(dto.Inlcude)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false) ??
            throw new AroGroupNotFoundException(dto.Id.ToString());

        var groupDto = new GroupDto(
            response.Id,
            response.GroupName,
            response.AddressLine1,
            response.AddressLine2,
            response.City,
            response.PostalCode,
            response.Country,
            response.Logo,
            response.PrimaryContactId,
            response.IsActive
        );

        return new GetGroupResponse(groupDto);
    }

    public async Task<PatchGroupResponse> PatchGroup(
        PatchGroupDto group,
        CancellationToken cancellationToken = default
        )
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.PatchGroup], cancellationToken);

        var existingGroup = await groupRepository.GetById(group.Id)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false)
            ?? throw new AroGroupNotFoundException(group.Id.ToString());

        if (group.PrimaryContactId.HasValue)
        {
            _ = await userRepository
                .GetById(group.PrimaryContactId.Value)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false)
                ?? throw new AroUserNotFoundException(group.PrimaryContactId.Value.ToString());

            existingGroup.PrimaryContactId = group.PrimaryContactId.Value;
        }

        if (group.IsActive.HasValue)
            existingGroup.IsActive = group.IsActive.Value;

        groupRepository.Update(existingGroup);
        await repository.SaveChanges(cancellationToken).ConfigureAwait(false);

        return new PatchGroupResponse(
            existingGroup.Id,
            existingGroup.GroupName,
            existingGroup.AddressLine1,
            existingGroup.AddressLine2,
            existingGroup.City,
            existingGroup.PostalCode,
            existingGroup.Country,
            existingGroup.Logo,
            existingGroup.PrimaryContactId,
            existingGroup.IsActive
            );
    }

    public async Task<DeleteGroupResponse> DeleteGroup(
        DeleteGroupDto dto,
        CancellationToken cancellationToken = default
        )
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.DeleteGroup], cancellationToken);

        var query = groupRepository.GetById(dto.Id);

        var group = await query
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false) ??
            throw new AroGroupNotFoundException(dto.Id.ToString());

        groupRepository.Delete(group);
        await repository.SaveChanges(cancellationToken).ConfigureAwait(false);

        return new DeleteGroupResponse(dto.Id);
    }
}
