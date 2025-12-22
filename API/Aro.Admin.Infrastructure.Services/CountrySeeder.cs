using Aro.Admin.Application.Services.CountriesSeeder;
using Aro.Common.Application.Services.Country;
using Aro.Common.Application.Services.Serializer;
using Aro.Common.Domain.Entities;
using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;

namespace Aro.Admin.Infrastructure.Services;

public class CountrySeeder(ErrorCodes errorCodes, ISerializer serializer, ICountryService countryService) : ICountrySeeder
{
    public async Task Seed(string jsonFile, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(jsonFile))
        {
            throw new FileNotFoundException(errorCodes.FILE_NOT_FOUND_ERROR, $"Not found: {jsonFile}");
        }

        var json = await File.ReadAllTextAsync(jsonFile, cancellationToken).ConfigureAwait(false);

        var dataToSeed = serializer.Deserialize<List<Country>>(json)
            ?? throw new AroException(errorCodes.DESERIALIZATION_ERROR, "Failed to parse metadata.");

        var serviceResponse = await countryService.Create([.. dataToSeed.Select(c => new CountryDto(c.Name, c.OfficialName, c.ISO2, c.PostalCodeRegex, c.PhoneCountryCode, c.PhoneNumberRegex))], cancellationToken).ConfigureAwait(false);
    }
}

file record Country(string Name, string OfficialName, string ISO2, string PostalCodeRegex, string PhoneCountryCode, string PhoneNumberRegex);
