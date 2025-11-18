using Aro.Booking.Application.Services.Property;
using Aro.Booking.Domain.Entities;
using Aro.Booking.Domain.Shared.Exceptions;
using Aro.Common.Application.Repository;
using Aro.Common.Application.Services.LogManager;
using Microsoft.EntityFrameworkCore;
using BookingRepositoryManager = Aro.Booking.Application.Repository.IRepositoryManager;

namespace Aro.Booking.Infrastructure.Services;

public class PropertyService(
    BookingRepositoryManager repositoryManager,
    IUnitOfWork unitOfWork,
    ILogManager<PropertyService> logger
) : IPropertyService
{
    public async Task<CreatePropertyResponse> CreateProperty(
        CreatePropertyDto propertyDto,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName} for property: {PropertyName}", nameof(CreateProperty), propertyDto.PropertyName);

        var existingProperty = await repositoryManager.PropertyRepository
            .GetByNameAndGroupId(propertyDto.PropertyName, propertyDto.GroupId)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (existingProperty != null)
        {
            logger.LogWarn("Property with name '{PropertyName}' already exists for group '{GroupId}'",
                propertyDto.PropertyName, propertyDto.GroupId?.ToString() ?? string.Empty);
            throw new PropertyAlreadyExistsException(
                propertyDto.PropertyName,
                propertyDto.GroupId?.ToString() ?? "null");
        }

        var property = new Property
        {
            Id = Guid.NewGuid(),
            GroupId = propertyDto.GroupId,
            PropertyName = propertyDto.PropertyName,
            PropertyTypes = propertyDto.PropertyTypes,
            StarRating = propertyDto.StarRating,
            Currency = propertyDto.Currency,
            Description = propertyDto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null,
            IsActive = true
        };

        await repositoryManager.PropertyRepository.Create(property, cancellationToken).ConfigureAwait(false);
        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

        logger.LogInfo("Property created successfully with Id: {PropertyId}", property.Id);

        return new CreatePropertyResponse(
            property.Id,
            property.GroupId,
            property.PropertyName,
            propertyDto.PropertyTypes,
            property.StarRating,
            property.Currency,
            property.Description
        );
    }

    public async Task<GetPropertyResponse> GetPropertyById(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName} for propertyId: {PropertyId}", nameof(GetPropertyById), propertyId);

        var property = await repositoryManager.PropertyRepository
            .GetById(propertyId)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (property == null)
        {
            logger.LogWarn("Property with Id '{PropertyId}' not found", propertyId);
            throw new PropertyNotFoundException(propertyId.ToString());
        }

        logger.LogDebug("Property with Id '{PropertyId}' retrieved successfully", propertyId);

        return new GetPropertyResponse(
            property.Id,
            property.GroupId,
            property.PropertyName,
            property.PropertyTypes,
            property.StarRating,
            property.Currency,
            property.Description,
            property.CreatedAt,
            property.UpdatedAt,
            property.IsActive
        );
    }

    public async Task<List<GetPropertyResponse>> GetPropertiesByGroupId(
        Guid groupId,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName} for groupId: {GroupId}", nameof(GetPropertiesByGroupId), groupId);

        var properties = await repositoryManager.PropertyRepository
            .GetByGroupId(groupId)
            .OrderBy(p => p.PropertyName)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        logger.LogDebug("Retrieved {Count} properties for group '{GroupId}'", properties.Count, groupId);

        return properties.Select(property => new GetPropertyResponse(
            property.Id,
            property.GroupId,
            property.PropertyName,
            property.PropertyTypes,
            property.StarRating,
            property.Currency,
            property.Description,
            property.CreatedAt,
            property.UpdatedAt,
            property.IsActive
        )).ToList();
    }
}
