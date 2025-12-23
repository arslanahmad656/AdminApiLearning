using Aro.Booking.Application.Repository;
using Aro.Booking.Application.Services.Amenity;
using Aro.Booking.Domain.Entities;
using Aro.Booking.Domain.Shared.Exceptions;
using Aro.Common.Application.Repository;
using Aro.Common.Application.Services.Authorization;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.UniqueIdGenerator;
using Aro.Common.Domain.Shared;
using Aro.Common.Infrastructure.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Aro.Booking.Infrastructure.Services;

public partial class AmenityService(
    Application.Repository.IRepositoryManager bookingRepository,
    IUnitOfWork unitOfWork,
    ILogManager<AmenityService> logger,
    IAuthorizationService authorizationService,
    IUniqueIdGenerator idGenerator
) : IAmenityService
{
    private readonly IAmenityRepository amenityRepository = bookingRepository.AmenityRepository;

    public async Task<CreateAmenityResponse> CreateAmenity(CreateAmenityDto amenity, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.CreateAmenity], cancellationToken);

        var AmenityEntity = new Amenity
        {
            Id = idGenerator.Generate(),
            Name = amenity.Name,
        };

        await amenityRepository.Create(AmenityEntity, cancellationToken).ConfigureAwait(false);

        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

        return new CreateAmenityResponse(new(AmenityEntity.Id, AmenityEntity.Name));
    }

    public async Task<GetAmenitiesResponse> GetAmenities(
        GetAmenitiesDto query,
        CancellationToken cancellationToken = default
        )
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.GetAmenities], cancellationToken);

        var baseQuery = amenityRepository.GetAll();

        baseQuery = baseQuery
            .IncludeElements(query.Include ?? string.Empty)
            .SortBy(query.SortBy, query.Ascending);

        var totalCount = await baseQuery
            .CountAsync(cancellationToken)
            .ConfigureAwait(false);

        var pagedQuery = baseQuery
            .Paginate(query.Page, query.PageSize);

        var amenityDtos = await pagedQuery
            .Select(r => new AmenityDto(
                r.Id,
                r.Name
            ))
            .ToListAsync(cancellationToken);

        return new GetAmenitiesResponse(amenityDtos, totalCount);
    }

    public async Task<GetAmenityResponse> GetAmenityById(
        GetAmenityDto dto,
        CancellationToken cancellationToken = default
        )
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.GetAmenity], cancellationToken);

        var query = await amenityRepository.GetById(dto.Id)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false) ??
            throw new AroAmenityNotFoundException(dto.Id.ToString());

        var amenityDto = new AmenityDto(
            query.Id,
            query.Name
        );

        return new GetAmenityResponse(amenityDto);
    }

    public async Task<PatchAmenityResponse> PatchAmenity(
        PatchAmenityDto dto,
        CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.PatchAmenity], cancellationToken);

        var _amenity = dto.Amenity;

        var existingAmenity = await amenityRepository.GetById(_amenity.Id)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false) ??
            throw new AroAmenityNotFoundException(_amenity.Id.ToString());

        _amenity.Name.PatchIfNotNull(v => existingAmenity.Name = v, logger, nameof(existingAmenity.Name));

        amenityRepository.Update(existingAmenity);
        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

        return new PatchAmenityResponse(new(_amenity.Id, _amenity.Name));
    }

    public async Task<DeleteAmenityResponse> DeleteAmenity(DeleteAmenityDto amenity, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.DeleteAmenity], cancellationToken);

        var existingAmenity = await amenityRepository.GetById(amenity.Id)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false) ??
            throw new AroAmenityNotFoundException(amenity.Id.ToString());

        amenityRepository.Delete(existingAmenity);

        return new DeleteAmenityResponse(amenity.Id);

    }
}
