using Aro.Booking.Application.Services.Room;
using Aro.Booking.Domain.Entities;
using Aro.Booking.Domain.Shared.Exceptions;
using Aro.Common.Application.Repository;
using Aro.Common.Application.Services.Authorization;
using Aro.Common.Application.Services.FileResource;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.UniqueIdGenerator;
using Aro.Common.Domain.Shared;
using Aro.Common.Infrastructure.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

using BookingRepositoryManager = Aro.Booking.Application.Repository.IRepositoryManager;

namespace Aro.Booking.Infrastructure.Services;

public partial class RoomService(
    BookingRepositoryManager repositoryManager,
    IUnitOfWork unitOfWork,
    ILogManager<PropertyService> logger,
    IUniqueIdGenerator idGenerator,
    IAuthorizationService authorizationService,
    IFileResourceService fileService
) : IRoomService
{
    public async Task<CreateRoomResponse> CreateRoom(CreateRoomDto room, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName} for room: {RoomName}", nameof(CreateRoom), room.RoomName);

        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.CreateRoom], cancellationToken);

        var existingProperty = await repositoryManager.PropertyRepository.GetById(room.PropertyId)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (existingProperty is null)
        {
            logger.LogWarn("Property with Id '{PropertyId}' does not exist", room.PropertyId.ToString() ?? string.Empty);
            throw new PropertyNotFoundException(room.PropertyId.ToString());
        }

        var existingRoom = await repositoryManager.RoomRepository.GetByRoomCode(room.RoomCode)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (existingRoom is not null)
        {
            logger.LogWarn("Room with code '{RoomCode}' already exists for room '{RoomId}'",
                room.RoomCode, existingRoom.Id.ToString() ?? string.Empty);
            throw new RoomAlreadyExistsException(room.RoomCode, existingRoom.Id.ToString() ?? "null");
        }

        var maxDisplayOrder = await repositoryManager.RoomRepository.GetAll()
            .Where(r => r.PropertyId == room.PropertyId)
            .Select(r => (int?)r.DisplayOrder)
            .MaxAsync(cancellationToken)
            .ConfigureAwait(false) ?? -1;

        Guid _roomId = idGenerator.Generate();
        var RoomEntity = new Room
        {
            Id = _roomId,
            PropertyId = room.PropertyId,
            RoomName = room.RoomName,
            RoomCode = room.RoomCode,
            Description = room.Description,
            MaxOccupancy = room.MaxOccupancy,
            MaxAdults = room.MaxAdults,
            MaxChildren = room.MaxChildren,
            RoomSize = room.RoomSizeSQM,
            BedConfig = (Domain.Entities.BedConfiguration)room.BedConfig,
            IsActive = room.IsActive,
            DisplayOrder = maxDisplayOrder + 1,
            RoomAmenities = []
        };

        if (room.Amenities is not null)
        {
            logger.LogDebug("Now creating / linking amenities.");
            foreach (var amenity in room.Amenities)
            {
                var existingAnemity = await repositoryManager.AmenityRepository.GetByName(amenity)
                    .SingleOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);

                if (existingAnemity is not null)
                {
                    logger.LogDebug("Amenity with name '{Amenity}' already exists. Creating link to existing amenity.", amenity);

                    RoomEntity.RoomAmenities.Add(new RoomAmenity
                    {
                        RoomId = RoomEntity.Id,
                        AmenityId = existingAnemity.Id
                    });
                }
                else
                {
                    logger.LogDebug("Creating new amenity with name '{Amenity}'. Creating link to new amenity.", amenity);

                    var newAmenity = new Amenity
                    {
                        Id = idGenerator.Generate(),
                        Name = amenity
                    };

                    await repositoryManager.AmenityRepository.Create(newAmenity, cancellationToken).ConfigureAwait(false);
                    await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

                    RoomEntity.RoomAmenities.Add(new RoomAmenity
                    {
                        RoomId = RoomEntity.Id,
                        AmenityId = newAmenity.Id
                    });
                }
            }
        }

        await repositoryManager.RoomRepository.Create(RoomEntity, cancellationToken).ConfigureAwait(false);
        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

        logger.LogInfo("Room created successfully with Id: {RoomId}", _roomId);

        if (room.RoomImages is not null)
        {
            logger.LogDebug("Now creating files.");
            foreach (var fileData in room.RoomImages)
            {
                try
                {
                    var fileNameToUse = idGenerator.Generate().ToString("N");
                    var fileServiceResponse = await fileService.CreateFile(new(fileNameToUse, fileData.Content, $"File for room {room.RoomName}.", fileData.Name, null), cancellationToken);
                    await repositoryManager.RoomFilesRepository.Create(new()
                    {
                        FileId = fileServiceResponse.Id,
                        RoomId = _roomId,
                        OrderIndex = fileData.OrderIndex,
                        IsThumbnail = fileData.IsThumbnail
                    }, cancellationToken);

                    await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

                    logger.LogDebug($"Created file {fileData.Name}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while creating the file {FileName}", fileData.Name);
                }
            }
        }

        return new CreateRoomResponse(RoomEntity.Id, RoomEntity.RoomName);
    }

    public async Task<GetRoomsResponse> GetRooms(
        GetRoomsDto query,
        CancellationToken cancellationToken = default
        )
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.GetRooms], cancellationToken);

        var baseQuery = repositoryManager.RoomRepository.GetAll();

        if (query.PropertyId.HasValue)
        {
            baseQuery = baseQuery.Where(r => r.PropertyId == query.PropertyId.Value);
        }

        // Transform include parameter: filter out Images (handled separately) and map Amenities -> RoomAmenities
        var efInclude = query.Include ?? string.Empty;
        var includeElements = efInclude.Split(',')
            .Select(s => s.Trim())
            .Where(s => !s.Equals("Images", StringComparison.OrdinalIgnoreCase))
            .Select(s => s.Equals("Amenities", StringComparison.OrdinalIgnoreCase) ? "RoomAmenities.Amenity" : s);
        efInclude = string.Join(",", includeElements);

        baseQuery = baseQuery
            .IncludeElements(efInclude)
            .OrderBy(r => r.DisplayOrder);

        var totalCount = await baseQuery
            .CountAsync(cancellationToken)
            .ConfigureAwait(false);

        var pagedQuery = baseQuery
            .Paginate(query.Page, query.PageSize);

        var rooms = await pagedQuery.ToListAsync(cancellationToken);

        var includeParam = query.Include?.ToLowerInvariant() ?? string.Empty;
        var includeImages = includeParam.Contains("images");

        var roomDtos = new List<RoomDto>();
        foreach (var r in rooms)
        {
            List<RoomDto.RoomImageInfoDto>? images = null;
            if (includeImages)
            {
                var roomFiles = await repositoryManager.RoomFilesRepository.GetByRoomId(r.Id)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                images = roomFiles.Select(rf => new RoomDto.RoomImageInfoDto(
                    rf.Entity.FileId,
                    rf.Entity.OrderIndex,
                    rf.Entity.IsThumbnail
                )).ToList();
            }

            roomDtos.Add(new RoomDto(
                r.Id,
                r.RoomName,
                r.RoomCode,
                r.Description,
                r.MaxOccupancy,
                r.MaxAdults,
                r.MaxChildren,
                r.RoomSize,
                (Aro.Booking.Application.Services.Room.BedConfiguration)r.BedConfig,
                r.RoomAmenities?.Select(ra => ra.AmenityId).ToList(),
                r.IsActive,
                r.DisplayOrder,
                images
            ));
        }

        return new GetRoomsResponse(roomDtos, totalCount);
    }

    public async Task<GetRoomResponse> GetRoomById(
        GetRoomDto dto,
        CancellationToken cancellationToken = default
        )
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.GetRoom], cancellationToken);

        var query = repositoryManager.RoomRepository.GetById(dto.Id);

        // Transform include parameter: filter out Images (handled separately) and map Amenities -> RoomAmenities
        var efInclude = dto.Inlcude ?? string.Empty;
        var includeElements = efInclude.Split(',')
            .Select(s => s.Trim())
            .Where(s => !s.Equals("Images", StringComparison.OrdinalIgnoreCase))
            .Select(s => s.Equals("Amenities", StringComparison.OrdinalIgnoreCase) ? "RoomAmenities.Amenity" : s);
        efInclude = string.Join(",", includeElements);

        var response = await query
            .IncludeElements(efInclude)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false) ??
            throw new AroRoomNotFoundException(dto.Id.ToString());

        List<RoomDto.RoomImageInfoDto>? images = null;
        var includeParam = dto.Inlcude?.ToLowerInvariant() ?? string.Empty;
        if (includeParam.Contains("images"))
        {
            var roomFiles = await repositoryManager.RoomFilesRepository.GetByRoomId(dto.Id)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            images = roomFiles.Select(rf => new RoomDto.RoomImageInfoDto(
                rf.Entity.FileId,
                rf.Entity.OrderIndex,
                rf.Entity.IsThumbnail
            )).ToList();
        }

        var roomDto = new RoomDto(
            response.Id,
            response.RoomName,
            response.RoomCode,
            response.Description,
            response.MaxOccupancy,
            response.MaxAdults,
            response.MaxChildren,
            response.RoomSize,
            (Aro.Booking.Application.Services.Room.BedConfiguration)response.BedConfig,
            response.RoomAmenities?.Select(ra => ra.AmenityId).ToList(),
            response.IsActive,
            response.DisplayOrder,
            images
        );

        return new GetRoomResponse(roomDto);
    }

    public async Task<PatchRoomResponse> PatchRoom(
        PatchRoomDto room,
        CancellationToken cancellationToken = default
        )
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.PatchRoom], cancellationToken);

        var _room = room.Room;
        var existingRoom = await repositoryManager.RoomRepository.GetById(_room.Id)
            .Include(r => r.RoomAmenities) // Necessary for updating amenities
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false)
            ?? throw new AroRoomNotFoundException(_room.Id.ToString());

        _room.RoomName.PatchIfNotNull(v => existingRoom.RoomName = v, logger, nameof(existingRoom.RoomName));
        _room.RoomCode.PatchIfNotNull(v => existingRoom.RoomCode = v, logger, nameof(existingRoom.RoomCode));
        _room.Description.PatchIfNotNull(v => existingRoom.Description = v, logger, nameof(existingRoom.Description));
        _room.MaxOccupancy.PatchIfNotNull(v => existingRoom.MaxOccupancy = v, logger, nameof(existingRoom.MaxOccupancy));
        _room.MaxAdults.PatchIfNotNull(v => existingRoom.MaxAdults = v, logger, nameof(existingRoom.MaxAdults));
        _room.MaxChildren.PatchIfNotNull(v => existingRoom.MaxChildren = v, logger, nameof(existingRoom.MaxChildren));
        _room.RoomSizeSQM.PatchIfNotNull(v => existingRoom.RoomSize = v, logger, nameof(existingRoom.RoomSize));
        _room.BedConfig.PatchIfNotNull(v => existingRoom.BedConfig = (Domain.Entities.BedConfiguration)v, logger, nameof(existingRoom.BedConfig));
        _room.IsActive.PatchIfNotNull(v => existingRoom.IsActive = v, logger, nameof(existingRoom.IsActive));

        if (_room.AmenityIds is not null)
        {
            existingRoom.RoomAmenities.Clear(); // Clear existing amenities included in above query
            foreach (var amenityId in _room.AmenityIds)
            {
                _ = await repositoryManager.AmenityRepository.GetById(amenityId)
                    .SingleOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false) ?? throw new AroAmenityNotFoundException(amenityId.ToString());

                existingRoom.RoomAmenities.Add(new RoomAmenity
                {
                    RoomId = existingRoom.Id,
                    AmenityId = amenityId
                });
            }
        }

        repositoryManager.RoomRepository.Update(existingRoom);
        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

        return new PatchRoomResponse(new(
            _room.Id,
            _room.RoomName,
            _room.RoomCode,
            _room.Description,
            _room.MaxOccupancy,
            _room.MaxAdults,
            _room.MaxChildren,
            _room.RoomSizeSQM,
            _room.RoomSizeSQFT,
            (Application.Services.Room.BedConfiguration?)_room.BedConfig,
            _room.AmenityIds,
            _room.IsActive
            ));
    }

    public async Task<DeleteRoomResponse> DeleteRoom(
        DeleteRoomDto dto,
        CancellationToken cancellationToken = default
        )
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.DeleteRoom], cancellationToken);

        var query = repositoryManager.RoomRepository.GetById(dto.Id);

        var room = await query
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false) ??
            throw new AroRoomNotFoundException(dto.Id.ToString());

        repositoryManager.RoomRepository.Delete(room);
        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

        return new DeleteRoomResponse(dto.Id);
    }

    public async Task<ActivateRoomResponse> ActivateRoom(
        ActivateRoomDto dto,
        CancellationToken cancellationToken = default
        )
    {
        logger.LogDebug("Starting {MethodName} for roomId: {RoomId}", nameof(ActivateRoom), dto.Id);

        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.PatchRoom], cancellationToken);

        var room = await repositoryManager.RoomRepository.GetById(dto.Id)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false)
            ?? throw new AroRoomNotFoundException(dto.Id.ToString());

        if (!room.IsActive)
        {
            room.IsActive = true;
            repositoryManager.RoomRepository.Update(room);
            await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);
        }

        logger.LogInfo("Room activated successfully with Id: {RoomId}", room.Id);

        return new ActivateRoomResponse(room.Id, room.IsActive);
    }

    public async Task<DeactivateRoomResponse> DeactivateRoom(
        DeactivateRoomDto dto,
        CancellationToken cancellationToken = default
        )
    {
        logger.LogDebug("Starting {MethodName} for roomId: {RoomId}", nameof(DeactivateRoom), dto.Id);

        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.PatchRoom], cancellationToken);

        var room = await repositoryManager.RoomRepository.GetById(dto.Id)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false)
            ?? throw new AroRoomNotFoundException(dto.Id.ToString());

        if (room.IsActive)
        {
            room.IsActive = false;
            repositoryManager.RoomRepository.Update(room);
            await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);
        }

        logger.LogInfo("Room deactivated successfully with Id: {RoomId}", room.Id);

        return new DeactivateRoomResponse(room.Id, room.IsActive);
    }

    public async Task<ReorderRoomsResponse> ReorderRooms(
        ReorderRoomsDto dto,
        CancellationToken cancellationToken = default
        )
    {
        logger.LogDebug("Starting {MethodName} for propertyId: {PropertyId}", nameof(ReorderRooms), dto.PropertyId);

        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.PatchRoom], cancellationToken);

        var rooms = await repositoryManager.RoomRepository.GetAll()
            .Where(r => r.PropertyId == dto.PropertyId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var roomDict = rooms.ToDictionary(r => r.Id);
        int updatedCount = 0;

        foreach (var orderItem in dto.RoomOrders)
        {
            if (roomDict.TryGetValue(orderItem.RoomId, out var room))
            {
                if (room.DisplayOrder != orderItem.DisplayOrder)
                {
                    room.DisplayOrder = orderItem.DisplayOrder;
                    repositoryManager.RoomRepository.Update(room);
                    updatedCount++;
                }
            }
        }

        if (updatedCount > 0)
        {
            await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);
        }

        logger.LogInfo("Reordered {UpdatedCount} rooms for property {PropertyId}", updatedCount, dto.PropertyId);

        return new ReorderRoomsResponse(true, updatedCount);
    }
}
