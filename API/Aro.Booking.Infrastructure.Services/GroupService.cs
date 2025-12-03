using Aro.Booking.Application.Repository;
using Aro.Booking.Application.Services.Group;
using Aro.Booking.Domain.Entities;
using Aro.Booking.Domain.Shared.Exceptions;
using Aro.Common.Application.Repository;
using Aro.Common.Application.Services.Authorization;
using Aro.Common.Application.Services.UniqueIdGenerator;
using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;
using Aro.Common.Infrastructure.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Aro.Booking.Infrastructure.Services;

public partial class GroupService(
    Application.Repository.IRepositoryManager bookingRepository,
    Common.Application.Repository.IRepositoryManager commonRepository,
    IUnitOfWork unitOfWork,
    IUniqueIdGenerator idGenerator,
    IAuthorizationService authorizationService
) : IGroupService
{
    private readonly IGroupRepository groupRepository = bookingRepository.GroupRepository;
    private readonly IUserRepository userRepository = commonRepository.UserRepository;

    public async Task<CreateGroupResponse> CreateGroup(CreateGroupDto group, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.CreateGroup], cancellationToken);

        var query = userRepository.GetById(group.ContactId);
        _ = await query
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false) ??
            throw new AroUserNotFoundException(group.ContactId.ToString());

        var GroupEntity = new Group
        {
            Id = idGenerator.Generate(),
            GroupName = group.GroupName,
            PrimaryContactId = group.ContactId,
            Address = new()
            {
                AddressLine1 = group.AddressLine1,
                AddressLine2 = group.AddressLine2,
                City = group.City,
                Country = group.Country,
                PostalCode = group.PostalCode,
            },
            Logo = group.Logo,
            IsActive = group.IsActive
        };

        await groupRepository.Create(GroupEntity, cancellationToken).ConfigureAwait(false);

        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

        return new CreateGroupResponse(GroupEntity.Id, GroupEntity.GroupName);
    }

    public async Task<GetGroupsResponse> GetGroups(
        GetGroupsDto query,
        CancellationToken cancellationToken = default
        )
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.GetGroups], cancellationToken);

        var baseQuery = groupRepository.GetAll();

        if (query.Filter is char c && c is >= 'A' and <= 'Z' or >= 'a' and <= 'z')
        {
            baseQuery = baseQuery.Where(e => EF.Functions.Like(
                EF.Property<string>(e, "GroupName"), $"{query.Filter}%"));
        }

        baseQuery = baseQuery
            .IncludeElements(query.Include)
            .SortBy(query.SortBy, query.Ascending);

        var totalCount = await baseQuery
            .CountAsync(cancellationToken)
            .ConfigureAwait(false);

        var pagedQuery = baseQuery
            .Paginate(query.Page, query.PageSize);

        var groupDtos = await pagedQuery
            .Select(g => new GroupDto(
                g.Id,
                g.GroupName,
                g.Address.AddressLine1,
                g.Address.AddressLine2,
                g.Address.City,
                g.Address.PostalCode,
                g.Address.Country,
                g.Logo,
                g.PrimaryContact.Id,
                g.PrimaryContact.DisplayName,
                g.PrimaryContact.Email,
                g.IsActive
            ))
            .ToListAsync(cancellationToken);

        return new GetGroupsResponse(groupDtos, totalCount);
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
            response.Address.AddressLine1,
            response.Address.AddressLine2,
            response.Address.City,
            response.Address.PostalCode,
            response.Address.Country,
            response.Logo,
            response.PrimaryContact.Id,
            response.PrimaryContact?.DisplayName,
            response.PrimaryContact?.Email,
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

        if (group.ContactId.HasValue)
        {
            _ = await userRepository
                .GetById(group.ContactId.Value)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false)
                ?? throw new AroUserNotFoundException(group.ContactId.Value.ToString());

            existingGroup.PrimaryContactId = group.ContactId.Value;
        }

        group.GroupName.PatchIfNotNull(v => existingGroup.GroupName = v);
        group.AddressLine1.PatchIfNotNull(v => existingGroup.Address.AddressLine1 = v);
        group.AddressLine2.PatchIfNotNull(v => existingGroup.Address.AddressLine2 = v);
        group.City.PatchIfNotNull(v => existingGroup.Address.City = v);
        group.Country.PatchIfNotNull(v => existingGroup.Address.Country = v);
        group.IsActive.PatchIfNotNull(v => existingGroup.IsActive = v);

        groupRepository.Update(existingGroup);
        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

        return new PatchGroupResponse(
            group.Id,
            group.GroupName,
            group.AddressLine1,
            group.AddressLine2,
            group.City,
            group.PostalCode,
            group.Country,
            group.Logo,
            group.ContactId,
            group.IsActive
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
        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

        return new DeleteGroupResponse(dto.Id);
    }
}
