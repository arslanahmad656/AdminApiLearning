using Aro.Common.Application.Repository;
using Aro.Common.Application.Services.Country;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.Serializer;
using Aro.Common.Application.Services.UniqueIdGenerator;
using Aro.Common.Domain.Entities;
using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Aro.Common.Infrastructure.Services;

public partial class CountryService(IRepositoryManager repositoryManager, ErrorCodes errorCodes, ILogManager<CountryService> logger, IUniqueIdGenerator idGenerator, IUnitOfWork unitOfWork) : ICountryService
{
    public async Task<List<CountryResponse>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            var entities = await repositoryManager.CountryRepository.GetAll()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

            var response = entities.Select(e => new CountryResponse
            (
                e.Id,
                e.Name,
                e.OfficialName,
                e.ISO2,
                e.PostalCodeRegex,
                e.PhoneCountryCode,
                e.PhoneNumberRegex
            )).ToList();

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting the countries.");
            throw new AroException(errorCodes.COUNTRY_RETRIEVAL_ERROR, "Error occurred while retrieving the countries.");
        }
    }

    public async Task<List<Guid>> Create(IEnumerable<CountryDto> countries, CancellationToken cancellationToken = default)
    {
        try
        {
            var ids = new List<Guid>();
            foreach (var country in countries)
            {
                var id = idGenerator.Generate();
                var entity = new Country
                {
                    Id = id,
                    Name = country.Name,
                    OfficialName = country.OfficialName,
                    ISO2 = country.ISO2,
                    PostalCodeRegex = country.PostalCodeRegex,
                    PhoneCountryCode = country.PhoneCountryCode,
                    PhoneNumberRegex = country.PhoneNumberRegex
                };

                ids.Add(id);

                await repositoryManager.CountryRepository.Create(entity, cancellationToken).ConfigureAwait(false);
            }

            await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

            return ids;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating countries.");

            throw new AroException(errorCodes.COUNTRY_CREATION_ERROR, "An error occurred while creating the countries.");
        }
    }
}
