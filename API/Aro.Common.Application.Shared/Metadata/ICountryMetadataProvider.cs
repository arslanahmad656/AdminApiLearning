namespace Aro.Common.Application.Shared.Metadata;

public interface ICountryMetadataProvider
{
    IReadOnlyCollection<CountryMetadata> Countries { get; }
}
