namespace Aro.Common.Application.Services.Metadata;

public interface ICountryMetadataService
{
    IReadOnlyCollection<CountryMetadata> Countries { get; }
}
