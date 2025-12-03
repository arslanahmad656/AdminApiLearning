using Aro.Booking.Application.Repository;
using Aro.Booking.Application.Services.Room;
using Aro.Booking.Domain.Entities;
using Aro.Booking.Domain.Shared.Exceptions;
using Aro.Common.Application.Repository;
using Aro.Common.Application.Services.Authorization;
using Aro.Common.Application.Services.UniqueIdGenerator;
using Aro.Common.Domain.Shared;
using Aro.Common.Infrastructure.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Aro.Booking.Infrastructure.Services;

public partial class RoomService(
    Application.Repository.IRepositoryManager bookingRepository,
    IUnitOfWork unitOfWork,
    IUniqueIdGenerator idGenerator,
    IAuthorizationService authorizationService
) : IRoomService
{
    private readonly IRoomRepository roomRepository = bookingRepository.RoomRepository;
    private readonly IAmenityRepository amenityRepository = bookingRepository.AmenityRepository;

    public async Task<CreateRoomResponse> CreateRoom(CreateRoomDto room, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.CreateRoom], cancellationToken);

        var existingRoom = await roomRepository.GetByRoomCode(room.RoomCode)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (existingRoom is not null)
        {
            throw new RoomAlreadyExistsException(room.RoomCode, existingRoom.Id.ToString() ?? "null");
        }

        var RoomEntity = new Room
        {
            Id = idGenerator.Generate(),
            RoomName = room.RoomName,
            RoomCode = room.RoomCode,
            Description = room.Description,
            MaxOccupancy = room.MaxOccupancy,
            MaxAdults = room.MaxAdults,
            MaxChildren = room.MaxChildren,
            RoomSize = room.RoomSizeSQM ?? 0,
            BedConfig = (Domain.Entities.BedConfiguration)room.BedConfig,
            IsActive = room.IsActive,
            RoomAmenities = []
        };

        if (room.AmenityIds is not null)
        {
            foreach (var amenityId in room.AmenityIds)
            {
                _ = await amenityRepository.GetById(amenityId)
                    .SingleOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false) ?? throw new AroAmenityNotFoundException(amenityId.ToString());

                RoomEntity.RoomAmenities.Add(new RoomAmenity
                {
                    RoomId = RoomEntity.Id,
                    AmenityId = amenityId
                });
            }
        }

        await roomRepository.Create(RoomEntity, cancellationToken).ConfigureAwait(false);

        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

        return new CreateRoomResponse(RoomEntity.Id, RoomEntity.RoomName);
    }

    public async Task<GetRoomsResponse> GetRooms(
        GetRoomsDto query,
        CancellationToken cancellationToken = default
        )
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.GetRooms], cancellationToken);

        var baseQuery = roomRepository.GetAll();

        baseQuery = baseQuery
            .IncludeElements(query.Include ?? string.Empty)
            .SortBy(query.SortBy, query.Ascending);

        var totalCount = await baseQuery
            .CountAsync(cancellationToken)
            .ConfigureAwait(false);

        var pagedQuery = baseQuery
            .Paginate(query.Page, query.PageSize);

        var roomDtos = await pagedQuery
            .Select(r => new RoomDto(
                r.Id,
                r.RoomName,
                r.RoomCode,
                r.Description,
                r.MaxOccupancy,
                r.MaxAdults,
                r.MaxChildren,
                r.RoomSize,
                (Aro.Booking.Application.Services.Room.BedConfiguration)r.BedConfig,
                r.RoomAmenities.Select(ra => ra.AmenityId).ToList(),
                r.IsActive
            ))
            .ToListAsync(cancellationToken);

        return new GetRoomsResponse(roomDtos, totalCount);
    }

    public async Task<GetRoomResponse> GetRoomById(
        GetRoomDto dto,
        CancellationToken cancellationToken = default
        )
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.GetRoom], cancellationToken);

        var query = roomRepository.GetById(dto.Id);

        var response = await query
            .IncludeElements(dto.Inlcude ?? string.Empty)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false) ??
            throw new AroRoomNotFoundException(dto.Id.ToString());

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
            [.. response.RoomAmenities.Select(ra => ra.AmenityId)],
            response.IsActive
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
        var existingRoom = await roomRepository.GetById(_room.Id)
            .Include(r => r.RoomAmenities) // Necessary for updating amenities
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false)
            ?? throw new AroRoomNotFoundException(_room.Id.ToString());

        _room.RoomName.PatchIfNotNull(v => existingRoom.RoomName = v);
        _room.RoomCode.PatchIfNotNull(v => existingRoom.RoomCode = v);
        _room.Description.PatchIfNotNull(v => existingRoom.Description = v);
        _room.MaxOccupancy.PatchIfNotNull(v => existingRoom.MaxOccupancy = v);
        _room.MaxAdults.PatchIfNotNull(v => existingRoom.MaxAdults = v);
        _room.MaxChildren.PatchIfNotNull(v => existingRoom.MaxChildren = v);
        _room.RoomSizeSQM.PatchIfNotNull(v => existingRoom.RoomSize = v);
        _room.BedConfig.PatchIfNotNull(v => existingRoom.BedConfig = (Domain.Entities.BedConfiguration)v);
        _room.IsActive.PatchIfNotNull(v => existingRoom.IsActive = v);

        if (_room.AmenityIds is not null)
        {
            existingRoom.RoomAmenities.Clear(); // Clear existing amenities included in above query
            foreach (var amenityId in _room.AmenityIds)
            {
                _ = await amenityRepository.GetById(amenityId)
                    .SingleOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false) ?? throw new AroAmenityNotFoundException(amenityId.ToString());

                existingRoom.RoomAmenities.Add(new RoomAmenity
                {
                    RoomId = existingRoom.Id,
                    AmenityId = amenityId
                });
            }
        }

        roomRepository.Update(existingRoom);
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

        var query = roomRepository.GetById(dto.Id);

        var room = await query
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false) ??
            throw new AroRoomNotFoundException(dto.Id.ToString());

        roomRepository.Delete(room);
        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

        return new DeleteRoomResponse(dto.Id);
    }
}
