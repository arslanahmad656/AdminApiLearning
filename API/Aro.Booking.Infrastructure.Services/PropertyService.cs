using Aro.Booking.Application.Services.Property;
using Aro.Booking.Domain.Entities;
using Aro.Booking.Domain.Shared.Exceptions;
using Aro.Booking.Infrastructure.Shared.Options;
using Aro.Common.Application.Repository;
using Aro.Common.Application.Services.FileResource;
using Aro.Common.Application.Services.Hasher;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.UniqueIdGenerator;
using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;
using Aro.Common.Infrastructure.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using BookingRepositoryManager = Aro.Booking.Application.Repository.IRepositoryManager;
using CommonRepositoryManager = Aro.Common.Application.Repository.IRepositoryManager;

namespace Aro.Booking.Infrastructure.Services;

public class PropertyService(
    BookingRepositoryManager repositoryManager,
    CommonRepositoryManager commonRepositoryManager,
    IUnitOfWork unitOfWork,
    ILogManager<PropertyService> logger,
    IUniqueIdGenerator idGenerator,
    IFileResourceService fileService,
    IOptions<PropertySettings> propertySettings,
    IHasher hasher
) : IPropertyService
{
    private readonly PropertySettings propertySettings = propertySettings.Value;
    private readonly string propertyManagerRole = "PropertyManager";

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
            KeySellingPoints = propertyDto.KeySellingPoints !=null? string.Join(Constants.DatabaseStringSplitter, propertyDto.KeySellingPoints): null,
            MarketingTitle = propertyDto.MarketingTitle,
            MarketingDescription = propertyDto.MarketingDescription,
            UpdatedAt = null,
            IsActive = true
        };

        Group? group = null;
        var address = new Address
        {
            Id = idGenerator.Generate()
        };

        if (propertyDto.SetAddressSameAsGroupAddress)
        {
            group = await repositoryManager.GroupRepository
                .GetById(propertyDto.GroupId)
                .Include(g => g.Address)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false)
                ?? throw new AroGroupNotFoundException(propertyDto.GroupId.ToString());

            address.AddressLine1 = group.Address.AddressLine1;
            address.AddressLine2 = group.Address.AddressLine2;
            address.City = group.Address.City;
            address.Country = group.Address.Country;
            address.PostalCode = group.Address.PostalCode;
            address.PhoneNumber = group.Address.PhoneNumber;
        }
        else
        {
            address.AddressLine1 = propertyDto.AddressLine1;
            address.AddressLine2 = propertyDto.AddressLine2;
            address.City = propertyDto.City;
            address.Country = propertyDto.Country;
            address.PostalCode = propertyDto.PostalCode;
            address.PhoneNumber = propertyDto.PhoneNumber;
        }

        address.Website = propertyDto.Website;
        property.Address = address;

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
            var roletoAssign = this.propertyManagerRole;
            var propertyManagerRole = await commonRepositoryManager.RoleRepository.GetByName(roletoAssign, cancellationToken).ConfigureAwait(false)
                ?? throw new AroRoleNotFoundException(roletoAssign);

            property.Contact = new()
            {
                CreatedAt = DateTime.Now,
                DisplayName = propertyDto.ContactName,
                Email = propertyDto.ContactEmail,
                PasswordHash = hasher.Hash(propertySettings.DefaultContactPassword),
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

    public async Task<UpdatePropertyResponse> UpdateProperty(
        UpdatePropertyDto propertyDto,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName} for property: {PropertyId}", nameof(UpdateProperty), propertyDto.PropertyId);

        var property = await repositoryManager.PropertyRepository
            .GetById(propertyDto.PropertyId)
            .Include(p => p.Address)
            .Include(p => p.Contact)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (property == null)
        {
            logger.LogWarn("Property with Id '{PropertyId}' not found", propertyDto.PropertyId);
            throw new PropertyNotFoundException(propertyDto.PropertyId.ToString());
        }

        // Check if property name is being changed and if so, ensure it's unique
        if (property.PropertyName != propertyDto.PropertyName)
        {
            var existingProperty = await repositoryManager.PropertyRepository
                .GetByNameAndGroupId(propertyDto.PropertyName, propertyDto.GroupId)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (existingProperty != null && existingProperty.Id != propertyDto.PropertyId)
            {
                logger.LogWarn("Property with name '{PropertyName}' already exists for group '{GroupId}'",
                    propertyDto.PropertyName, propertyDto.GroupId.ToString());
                throw new PropertyAlreadyExistsException(
                    propertyDto.PropertyName,
                    propertyDto.GroupId.ToString());
            }
        }

        // Update property fields
        property.PropertyName = propertyDto.PropertyName;
        property.PropertyTypes = propertyDto.PropertyTypes.ToFlag();
        property.StarRating = propertyDto.StarRating;
        property.Currency = propertyDto.Currency;
        property.Description = propertyDto.Description;
        property.KeySellingPoints = propertyDto.KeySellingPoints != null
            ? string.Join(Constants.DatabaseStringSplitter, propertyDto.KeySellingPoints)
            : null;
        property.MarketingTitle = propertyDto.MarketingTitle;
        property.MarketingDescription = propertyDto.MarketingDescription;
        property.UpdatedAt = DateTime.UtcNow;

        var group = await repositoryManager.GroupRepository
                .GetById(propertyDto.GroupId)
                .Include(g => g.Address)
                .Include(g => g.PrimaryContact)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false)
                ?? throw new AroGroupNotFoundException(propertyDto.GroupId.ToString());

        if (propertyDto.SetAddressSameAsGroupAddress)
        {
            logger.LogDebug("Property {PropertyId} is sharing address with group. Creating new address.", property.Id);
            property.Address.AddressLine1 = group.Address.AddressLine1;
            property.Address.AddressLine2 = group.Address.AddressLine2;
            property.Address.City = group.Address.City;
            property.Address.Country = group.Address.Country;
            property.Address.PostalCode = group.Address.PostalCode;
            property.Address.PhoneNumber = group.Address.PhoneNumber;
            property.Address.Website = group.Address.Website;
        }
        else
        {
            property.Address.AddressLine1 = propertyDto.AddressLine1;
            property.Address.AddressLine2 = propertyDto.AddressLine2;
            property.Address.City = propertyDto.City;
            property.Address.Country = propertyDto.Country;
            property.Address.PostalCode = propertyDto.PostalCode;
            property.Address.PhoneNumber = propertyDto.PhoneNumber;
            property.Address.Website = propertyDto.Website;
        }

        var isContactSharedWithGroup = property.ContactId == group.PrimaryContactId;
        if (isContactSharedWithGroup)
        {
            logger.LogDebug("Property {PropertyId} is sharing contact with group. Creating new contact.", property.Id);

            if (propertyDto.SetContactSameAsGroupContact)
            {
                // In this case, we do nothing because property's contact is already set as same the group's contact
            }
            else
            {
                // in this case, we need to create a new contact and set that contact as the property's new contact
                // First check if a user with this email already exists
                var existingUser = await commonRepositoryManager.UserRepository
                    .GetByEmail(propertyDto.ContactEmail)
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);

                if (existingUser != null)
                {
                    // Use the existing user as the property contact
                    logger.LogDebug("User with email {Email} already exists. Using existing user as property contact.", propertyDto.ContactEmail);
                    property.ContactId = existingUser.Id;
                }
                else
                {
                    var roletoAssign = this.propertyManagerRole;
                    var propertyManagerRole = await commonRepositoryManager.RoleRepository.GetByName(roletoAssign, cancellationToken).ConfigureAwait(false)
                        ?? throw new AroRoleNotFoundException(roletoAssign);

                    property.Contact = new()
                    {
                        CreatedAt = DateTime.Now,
                        DisplayName = propertyDto.ContactName,
                        Email = propertyDto.ContactEmail,
                        PasswordHash = hasher.Hash(propertySettings.DefaultContactPassword),
                        UserRoles = [
                            new()
                        {
                            RoleId = propertyManagerRole.Id
                        }
                        ]
                    };
                }
            }
        }
        else
        {
            if (propertyDto.SetContactSameAsGroupContact)
            {
                // in this case, we just need to point the property contact to the group contact
                property.ContactId = group.PrimaryContactId;
            }
            else
            {
                // Check if email is changing and if new email already exists for a different user
                if (property.Contact.Email != propertyDto.ContactEmail)
                {
                    var existingUser = await commonRepositoryManager.UserRepository
                        .GetByEmail(propertyDto.ContactEmail)
                        .FirstOrDefaultAsync(cancellationToken)
                        .ConfigureAwait(false);

                    if (existingUser != null && existingUser.Id != property.ContactId)
                    {
                        // Email already exists for a different user, use that user as contact
                        logger.LogDebug("User with email {Email} already exists. Switching property contact to existing user.", propertyDto.ContactEmail);
                        property.ContactId = existingUser.Id;
                    }
                    else
                    {
                        // we can do in place update
                        property.Contact.DisplayName = propertyDto.ContactName;
                        property.Contact.Email = propertyDto.ContactEmail;
                    }
                }
                else
                {
                    // Email not changing, just update display name
                    property.Contact.DisplayName = propertyDto.ContactName;
                }
            }
        }

        repositoryManager.PropertyRepository.Update(property);
        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

        logger.LogInfo("Property updated successfully with Id: {PropertyId}", property.Id);

        // Handle file deletions if DeletedFileIds provided
        if (propertyDto.DeletedFileIds != null && propertyDto.DeletedFileIds.Any())
        {
            logger.LogDebug("Processing file deletions for property {PropertyId}. Files to delete: {FileCount}",
                property.Id, propertyDto.DeletedFileIds.Count);

            var existingFiles = await repositoryManager.PropertyFilesRepository
                .GetByPropertyId(property.Id)
                .ToListAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            foreach (var fileId in propertyDto.DeletedFileIds)
            {
                try
                {
                    var fileToDelete = existingFiles.FirstOrDefault(f => f.Entity.FileId == fileId);
                    if (fileToDelete != null)
                    {
                        logger.LogDebug("Deleting file {FileId} for property {PropertyId}", fileId, property.Id);
                        repositoryManager.PropertyFilesRepository.Delete(fileToDelete.Entity);
                        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);
                        logger.LogDebug("Deleted file {FileId}", fileId);
                    }
                    else
                    {
                        logger.LogWarn("File {FileId} not found for property {PropertyId}", fileId, property.Id);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while deleting file {FileId}", fileId);
                }
            }
        }

        // Handle file updates if new files provided
        if (propertyDto.Files != null && propertyDto.Files.Any())
        {
            logger.LogDebug("Processing file updates for property {PropertyId}", property.Id);

            // Get existing files
            var existingFiles = await repositoryManager.PropertyFilesRepository
                .GetByPropertyId(property.Id)
                .ToListAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            foreach (var fileData in propertyDto.Files)
            {
                try
                {
                    // Check if this file type already exists and remove it
                    var existingFile = existingFiles.FirstOrDefault(f => f.File?.Metadata == fileData.FileName);
                    if (existingFile != null)
                    {
                        logger.LogDebug("Removing existing file {FileName} for property {PropertyId}", fileData.FileName, property.Id);
                        repositoryManager.PropertyFilesRepository.Delete(existingFile.Entity);
                        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);
                    }

                    // Create new file
                    var fileNameToUse = idGenerator.Generate().ToString("N");
                    logger.LogDebug("Creating file {FileName} for property {PropertyName}. Storage name: {StorageName}",
                        fileData.FileName, property.PropertyName, fileNameToUse);

                    var fileServiceResponse = await fileService.CreateFile(
                        new(fileNameToUse, fileData.Content, $"File for property {property.PropertyName}.", fileData.FileName, null),
                        cancellationToken);

                    await repositoryManager.PropertyFilesRepository.Create(new()
                    {
                        FileId = fileServiceResponse.Id,
                        PropertyId = property.Id
                    }, cancellationToken);

                    await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

                    logger.LogDebug("Created file {FileName}", fileData.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while updating file {FileName}", fileData.FileName);
                }
            }
        }

        return new UpdatePropertyResponse(property.Id, property.GroupId);
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
            property.Address?.AddressLine1 ?? string.Empty,
            property.Address?.AddressLine2 ?? string.Empty,
            property.Address?.City ?? string.Empty,
            property.Address?.Country ?? string.Empty,
            property.Address?.PostalCode ?? string.Empty,
            property.Address?.PhoneNumber ?? string.Empty,
            property.Address?.Website ?? string.Empty,
            property.Contact?.DisplayName ?? string.Empty,
            property.Contact?.Email ?? string.Empty,
            property.KeySellingPoints?.Split(Constants.DatabaseStringSplitter).ToList() ?? new List<string>(),
            property.MarketingTitle,
            property.MarketingDescription,
            images.Where(i => i.File != null).ToDictionary(i => i.File.Metadata ?? string.Empty, i => i.File.Id),
            property.IsActive
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
            property.Address?.AddressLine1 ?? string.Empty,
            property.Address?.AddressLine2 ?? string.Empty,
            property.Address?.City ?? string.Empty,
            property.Address?.Country ?? string.Empty,
            property.Address?.PostalCode ?? string.Empty,
            property.Address?.PhoneNumber ?? string.Empty,
            property.Address?.Website ?? string.Empty,
            property.Contact?.DisplayName ?? string.Empty,
            property.Contact?.Email ?? string.Empty,
            property.KeySellingPoints?.Split(Constants.DatabaseStringSplitter).ToList() ?? new List<string>(),
            property.MarketingTitle,
            property.MarketingDescription,
            images.Where(i => i.File != null).ToDictionary(i => i.File.Metadata ?? string.Empty, i => i.File.Id),
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
            .Include(p => p.Address)
            .Include(p => p.Contact)
            .OrderBy(p => p.PropertyName)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        logger.LogDebug("Retrieved {Count} properties for group '{GroupId}'", properties.Count, groupId);

        var response = new List<GetPropertyResponse>();

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
                property.Address?.PhoneNumber ?? string.Empty,
                property.Address?.Website ?? string.Empty,
                property.Contact?.DisplayName ?? string.Empty,
                property.Contact?.Email ?? string.Empty,
                property.KeySellingPoints?.Split(Constants.DatabaseStringSplitter).ToList() ?? new List<string>(),
                property.MarketingTitle,
                property.MarketingDescription,
                images.ToDictionary(i => i.File?.Metadata ?? string.Empty, i => i.File?.Id ?? Guid.Empty),
                property.IsActive
            );

            response.Add(propertyResponse);
        }

        return response;
    }
}
