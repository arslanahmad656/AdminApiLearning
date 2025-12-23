
using Aro.Booking.Application.Repository;
using Aro.Booking.Application.Services.Group;
using Aro.Booking.Domain.Entities;
using Aro.Booking.Domain.Shared.Exceptions;
using Aro.Common.Application.Repository;
using Aro.Common.Application.Services.Authorization;
using Aro.Common.Application.Services.FileResource;
using Aro.Common.Application.Services.Hasher;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.RandomValueGenerator;
using Aro.Common.Application.Services.UniqueIdGenerator;
using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;
using Aro.Common.Infrastructure.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Aro.Booking.Infrastructure.Services;

public partial class GroupService(
    Application.Repository.IRepositoryManager bookingRepository,
    Common.Application.Repository.IRepositoryManager commonRepository,
    IUnitOfWork unitOfWork,
    ILogManager<GroupService> logger,
    IUniqueIdGenerator idGenerator,
    IHasher hasher,
    IRandomValueGenerator randomValueGenerator,
    IAuthorizationService authorizationService,
    IFileResourceService fileResourceService
) : IGroupService
{
    private readonly IGroupRepository groupRepository = bookingRepository.GroupRepository;
    private readonly IUserRepository userRepository = commonRepository.UserRepository;

    private const string primaryContactRole = "ClientAdmin";

    public async Task<CreateGroupResponse> CreateGroup(CreateGroupDto group, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName} for room: {GroupName}", nameof(CreateGroup), group.GroupName);

        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.CreateGroup], cancellationToken);

        bool userCreated = false;

        var existingUser = await userRepository.GetByEmail(group.PrimaryContact.Email)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        var _userId = existingUser?.Id ?? Guid.Empty;

        if (existingUser is not null) // Link to existing user if found
        {
            logger.LogWarn("User with email '{Email}' already exists. Linking existing user to new group.", group.PrimaryContact.Email);
            //throw new AroUserAlreadyExistsException(group.PrimaryContact.Email);
        }
        else if (existingUser is null)
        {
            logger.LogDebug("Attempting to create user.");

            var _user = group.PrimaryContact;

            var role = await commonRepository.RoleRepository.GetByName(primaryContactRole, cancellationToken);

            if (role is null)
            {
                logger.LogError("Role '{RoleName}' not found when creating primary contact user for group '{GroupName}'", primaryContactRole, group.GroupName);
                throw new AroRoleNotFoundException($"Role '{primaryContactRole}' not found");
            }

            var _Id = idGenerator.Generate();
            var hashedPassword = hasher.Hash(randomValueGenerator.GenerateString(12));

            await commonRepository.UserRepository.Create(new()
            {
                Id = _Id,
                Email = _user.Email,
                PasswordHash = hashedPassword,
                IsActive = false,
                CreatedAt = DateTime.Now,
                DisplayName = _user.Name,
                IsSystem = false,
                UserRoles =
                [
                    new()
                    {
                        UserId = _Id,
                        RoleId = role.Id
                    }
                ],
                ContactInfo = _user.PhoneNumber != null ? new()
                {
                    CountryCode = _user.CountryCode,
                    PhoneNumber = _user.PhoneNumber
                } : null,
            }, cancellationToken);

            _userId = _Id;
            userCreated = true;

            logger.LogDebug("User staged for creation with ID: {UserId}", _userId);
        }

        logger.LogDebug("Attempting to create group.");
        var _groupId = idGenerator.Generate();
        var groupEntity = new Group
        {
            Id = _groupId,
            GroupName = group.GroupName,
            PrimaryContactId = _userId,
            Address = new()
            {
                AddressLine1 = group.AddressLine1,
                AddressLine2 = group.AddressLine2,
                City = group.City,
                Country = group.Country,
                PostalCode = group.PostalCode,
            },
            IsActive = group.IsActive
        };

        logger.LogDebug("Uploading group logo.");
        var fileNameToUse = idGenerator.Generate().ToString("N");
        var fileServiceResponse = await fileResourceService.CreateFile(new(fileNameToUse, group.Logo.Content, "Group Logo", groupEntity.Id.ToString(), null), cancellationToken).ConfigureAwait(false);
        await bookingRepository.GroupFilesRepository.Create(new()
        {
            FileId = fileServiceResponse.Id,
            GroupId = _groupId,
        }, cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Setting group icon ID to: {IconId}", fileServiceResponse.Id);
        groupEntity.IconId = fileServiceResponse.Id;

        await groupRepository.Create(groupEntity, cancellationToken).ConfigureAwait(false);
        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

        if (userCreated)
        {
            logger.LogDebug("User {UserId} successfully created and committed.", _userId);
        }

        return new CreateGroupResponse(_groupId, groupEntity.GroupName);
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
                g.IconId,
                g.PrimaryContact.Id,
                g.PrimaryContact.DisplayName,
                g.PrimaryContact.Email,
                g.PrimaryContact.ContactInfo != null ? g.PrimaryContact.ContactInfo.CountryCode : null,
                g.PrimaryContact.ContactInfo != null ? g.PrimaryContact.ContactInfo.PhoneNumber : null,
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
            response.Address?.AddressLine1 ?? string.Empty,
            response.Address?.AddressLine2,
            response.Address?.City ?? string.Empty,
            response.Address?.PostalCode ?? string.Empty,
            response.Address?.Country ?? string.Empty,
            response.IconId,
            response.PrimaryContact?.Id ?? Guid.Empty,
            response.PrimaryContact?.DisplayName,
            response.PrimaryContact?.Email,
            response.PrimaryContact?.ContactInfo?.CountryCode,
            response.PrimaryContact?.ContactInfo?.PhoneNumber,
            response.IsActive
        );

        return new GetGroupResponse(groupDto);
    }

    public async Task<PatchGroupResponse> PatchGroup(
        PatchGroupDto group,
        CancellationToken cancellationToken = default
        )
    {
        logger.LogDebug("Starting {MethodName} for room: {GroupId}", nameof(PatchGroup), group.Id);

        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.PatchGroup], cancellationToken);

        var existingGroup = await groupRepository.GetById(group.Id)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (existingGroup == null)
        {
            logger.LogWarn("Group with Id '{GoupId}' does not exist.", group.Id.ToString());
            throw new AroGroupNotFoundException(group.Id.ToString());
        }

        logger.LogDebug("Attempting to patch group properties");

        if (group.ContactId.HasValue)
        {
            var existingUser = await userRepository
                .GetById(group.ContactId.Value)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (existingUser == null)
            {
                logger.LogWarn("User with Id '{UserId}' does not exist.", group.ContactId.Value.ToString());
                throw new AroUserNotFoundException(group.ContactId.Value.ToString());

            }

            logger.LogDebug("Patching property {PropertyName} with value: {Value}", nameof(existingGroup.PrimaryContactId), group.ContactId.Value);
            existingGroup.PrimaryContactId = group.ContactId.Value;
        }

        group.GroupName.PatchIfNotNull(v => existingGroup.GroupName = v, logger, nameof(group.GroupName));
        group.AddressLine1.PatchIfNotNull(v => existingGroup.Address.AddressLine1 = v, logger, nameof(group.AddressLine1));
        group.AddressLine2.PatchIfNotNull(v => existingGroup.Address.AddressLine2 = v, logger, nameof(group.AddressLine2));
        group.City.PatchIfNotNull(v => existingGroup.Address.City = v, logger, nameof(group.City));
        group.Country.PatchIfNotNull(v => existingGroup.Address.Country = v, logger, nameof(group.Country));
        group.IsActive.PatchIfNotNull(v => existingGroup.IsActive = v, logger, nameof(group.IsActive));

        if (group.Logo != null)
        {
            logger.LogDebug("Uploading new group logo.");
            var fileNameToUse = idGenerator.Generate().ToString("N");
            var fileServiceResponse = await fileResourceService.CreateFile(new(fileNameToUse, group.Logo.Content, "Group Logo", existingGroup.Id.ToString(), null), cancellationToken).ConfigureAwait(false);
            await bookingRepository.GroupFilesRepository.Create(new()
            {
                FileId = fileServiceResponse.Id,
                GroupId = existingGroup.Id,
            }, cancellationToken).ConfigureAwait(false);

            logger.LogDebug("Patching property {PropertyName} with value: {Value}", nameof(existingGroup.IconId), fileServiceResponse.Id);
            existingGroup.IconId = fileServiceResponse.Id;
        }

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
            existingGroup.IconId,
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

    public static string GenerateRandomString(int length = 32)
    {
        const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
        var bytes = RandomNumberGenerator.GetBytes(length);
        var chars = bytes.Select(b => validChars[b % validChars.Length]);
        return new string([.. chars]);
    }
}
