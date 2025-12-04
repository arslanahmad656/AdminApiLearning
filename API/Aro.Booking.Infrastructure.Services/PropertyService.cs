using Aro.Booking.Application.Services.Property;
using Aro.Booking.Domain.Entities;
using Aro.Booking.Domain.Shared.Exceptions;
using Aro.Common.Application.Repository;
using Aro.Common.Application.Services.FileResource;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.UniqueIdGenerator;
using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;
using Aro.Common.Infrastructure.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using BookingRepositoryManager = Aro.Booking.Application.Repository.IRepositoryManager;
using CommonRepositoryManager = Aro.Common.Application.Repository.IRepositoryManager;

namespace Aro.Booking.Infrastructure.Services;

public class PropertyService(
    BookingRepositoryManager repositoryManager,
    CommonRepositoryManager commonRepositoryManager,
    IUnitOfWork unitOfWork,
    ILogManager<PropertyService> logger,
    IUniqueIdGenerator idGenerator,
    IFileResourceService fileService
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
                propertyDto.PropertyName, propertyDto.GroupId.ToString() ?? string.Empty);
            throw new PropertyAlreadyExistsException(
                propertyDto.PropertyName,
                propertyDto.GroupId.ToString() ?? "null");
        }

        var property = new Property
        {
            Id = idGenerator.Generate(),
            GroupId = propertyDto.GroupId,
            PropertyName = propertyDto.PropertyName,
            PropertyTypes = propertyDto.PropertyTypes.ToFlag(),
            StarRating = propertyDto.StarRating,
            Currency = propertyDto.Currency,
            Description = propertyDto.Description,
            CreatedAt = DateTime.UtcNow,
            KeySellingPoints = string.Join(Constants.DatabaseStringSplitter, propertyDto.KeySellingPoints),
            MarketingTitle = propertyDto.MarketingTitle,
            MarketingDescription = propertyDto.MarketingDescription,
            UpdatedAt = null,
            IsActive = true
        };

        Group? group = null;
        if (propertyDto.SetAddressSameAsGroupAddress)
        {
            group = await repositoryManager.GroupRepository
                .GetById(propertyDto.GroupId)
                .Include(g => g.Address)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false)
                ?? throw new AroGroupNotFoundException(propertyDto.GroupId.ToString());

            property.AddressId = group.AddressId;
        }
        else
        {
            property.Address = new()
            {
                Id = idGenerator.Generate(),
                AddressLine1 = propertyDto.AddressLine2,
                AddressLine2 = propertyDto.AddressLine2,
                City = propertyDto.City,
                Country = propertyDto.Country,
                PostalCode = propertyDto.PostalCode,
                PhoneNumber = propertyDto.PhoneNumber,
                Website = propertyDto.Website
            };
        }

        if (propertyDto.SetContactSameAsGroupContact)
        {
            group ??= await repositoryManager.GroupRepository
                .GetById(propertyDto.GroupId)
                .Include(g => g.Address)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false)
                ?? throw new AroGroupNotFoundException(propertyDto.GroupId.ToString());
            property.ContactId = group.PrimaryContactId;
        }
        else
        {
            var roletoAssign = "PropertyManager";
            var propertyManagerRole = await commonRepositoryManager.RoleRepository.GetByName(roletoAssign, cancellationToken).ConfigureAwait(false)
                ?? throw new AroRoleNotFoundException(roletoAssign);

            property.Contact = new()
            {
                CreatedAt = DateTime.Now,
                DisplayName = propertyDto.ContactName,
                Email = propertyDto.ContactEmail,
                PasswordHash = "$2a$11$B51W4gxuG88sGqNkyDI9se0mYym2Zh9K1uTOP/7ATwzLVyj4/WGFy",
                UserRoles = [
                    new()
                    {
                        RoleId = propertyManagerRole.Id
                    }
                ]
            };
        }

        await repositoryManager.PropertyRepository.Create(property, cancellationToken).ConfigureAwait(false);
        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

        logger.LogInfo("Property created successfully with Id: {PropertyId}", property.Id);

        logger.LogDebug("Now creating files.");
        foreach (var fileData in propertyDto.Files)
        {
            try
            {
                var fileNameToUse = idGenerator.Generate().ToString("N");
                logger.LogDebug($"Creating file {fileData.FileName} for the property {propertyDto.PropertyName}. The file name on the storage will be {fileNameToUse}.");
                var fileServiceResponse = await fileService.CreateFile(new(fileNameToUse, fileData.Content, $"File for property {propertyDto.PropertyName}.", fileData.FileName, null), cancellationToken);
                await repositoryManager.PropertyFilesRepository.Create(new()
                {
                    FileId = fileServiceResponse.Id,
                    PropertyId = property.Id
                }, cancellationToken);

                await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

                logger.LogDebug($"Created file {fileData.FileName}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while creating the file {FileName}", fileData.FileName);
            }
        }

        return new CreatePropertyResponse(
            property.Id,
            property.GroupId,
            property.PropertyName
        );
    }

    public async Task<GetPropertyResponse> GetPropertyById(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName} for propertyId: {PropertyId}", nameof(GetPropertyById), propertyId);

        var property = await repositoryManager.PropertyRepository
            .GetById(propertyId)
            .Include(p => p.Contact)
            .Include(p => p.Address)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (property == null)
        {
            logger.LogWarn("Property with Id '{PropertyId}' not found", propertyId);
            throw new PropertyNotFoundException(propertyId.ToString());
        }

        logger.LogDebug("Property with Id '{PropertyId}' retrieved successfully", propertyId);

        logger.LogWarn("Fetching the images info.");
        var images = await repositoryManager.PropertyFilesRepository
            .GetByPropertyId(propertyId)
            .ToListAsync(cancellationToken: cancellationToken);
        logger.LogWarn("Fetched the images info.");

        return new GetPropertyResponse(
            property.Id,
            property.GroupId,
            property.PropertyName,
            property.PropertyTypes.SplitFlags().ToList(),
            property.StarRating,
            property.Currency,
            property.Description,
            property.Address.AddressLine1,
            property.Address.AddressLine2,
            property.Address.City,
            property.Address.Country,
            property.Address.PostalCode,
            property.Address.PhoneNumber,
            property.Address.Website,
            property.Contact.DisplayName,
            property.Contact.Email,
            property.KeySellingPoints.Split(Constants.DatabaseStringSplitter).ToList(),
            property.MarketingTitle,
            property.MarketingDescription,
            images.ToDictionary(i => i.File.Metadata, i => i.File.Id)
        );
    }

    public async Task<GetPropertyResponse> GetPropertyByGroupAndId(
        Guid groupId,
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName} for groupId: {GroupId}, propertyId: {PropertyId} ", nameof(GetPropertyById), groupId, propertyId);

        var property = await repositoryManager.PropertyRepository
            .GetByGroupAndId(groupId, propertyId)
            .Include(p => p.Address)
            .Include(p => p.Contact)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (property == null)
        {
            logger.LogWarn("Property with Id '{PropertyId}' not found", propertyId);
            throw new PropertyNotFoundException(propertyId.ToString());
        }

        logger.LogDebug("Property with Id '{PropertyId}' retrieved successfully", propertyId);

        logger.LogWarn("Fetching the images info.");
        var images = await repositoryManager.PropertyFilesRepository
            .GetByPropertyId(propertyId)
            .ToListAsync(cancellationToken: cancellationToken);
        logger.LogWarn("Fetched the images info.");

        return new GetPropertyResponse(
            property.Id,
            property.GroupId,
            property.PropertyName,
            property.PropertyTypes.SplitFlags().ToList(),
            property.StarRating,
            property.Currency,
            property.Description,
            property.Address.AddressLine1,
            property.Address.AddressLine2,
            property.Address.City,
            property.Address.Country,
            property.Address.PostalCode,
            property.Address.PhoneNumber,
            property.Address.Website,
            property.Contact.DisplayName,
            property.Contact.Email,
            property.KeySellingPoints.Split(Constants.DatabaseStringSplitter).ToList(),
            property.MarketingTitle,
            property.MarketingDescription,
            images.ToDictionary(i => i.File.Metadata, i => i.File.Id)
        );
    }

    public async Task<List<GetPropertyResponse>> GetPropertiesByGroupId(
        Guid groupId,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName} for groupId: {GroupId}", nameof(GetPropertiesByGroupId), groupId);

        var properties = await repositoryManager.PropertyRepository
            .GetByGroupId(groupId)
            .Include(p => p.Address)
            .Include(p => p.Contact)
            .OrderBy(p => p.PropertyName)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        logger.LogDebug("Retrieved {Count} properties for group '{GroupId}'", properties.Count, groupId);

        var response = new List<GetPropertyResponse>();

        // Process sequentially to avoid DbContext threading issues
        foreach (var property in properties)
        {
            logger.LogDebug("Fetching the images info for property id {PropertyId}", property.Id);
            var images = await repositoryManager.PropertyFilesRepository
                .GetByPropertyId(property.Id)
                .ToListAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            logger.LogDebug("Fetched the images info.");

            var propertyResponse = new GetPropertyResponse(
                property.Id,
                property.GroupId,
                property.PropertyName,
                property.PropertyTypes.SplitFlags().ToList(),
                property.StarRating,
                property.Currency,
                property.Description,
                property.Address?.AddressLine1 ?? string.Empty,
                property.Address?.AddressLine2 ?? string.Empty,
                property.Address?.City ?? string.Empty,
                property.Address?.Country ?? string.Empty,
                property.Address?.PostalCode ?? string.Empty,
                property.Address?.PhoneNumber,
                property.Address?.Website,
                property.Contact?.DisplayName ?? string.Empty,
                property.Contact?.Email ?? string.Empty,
                property.KeySellingPoints?.Split(Constants.DatabaseStringSplitter).ToList() ?? new List<string>(),
                property.MarketingTitle,
                property.MarketingDescription,
                images.ToDictionary(i => i.File?.Metadata ?? string.Empty, i => i.File?.Id ?? Guid.Empty)
            );

            response.Add(propertyResponse);
        }

        return response;
    }
}
