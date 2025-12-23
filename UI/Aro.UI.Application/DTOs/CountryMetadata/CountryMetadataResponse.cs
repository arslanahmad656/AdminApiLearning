namespace Aro.UI.Application.DTOs.CountryMetadata;

public sealed class CountryMetadataResponse
{
    public ICollection<CountryMetadata> Countries { get; init; } = [];
}
