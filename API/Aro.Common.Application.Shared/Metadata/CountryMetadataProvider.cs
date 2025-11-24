using System.Reflection;
using System.Text.Json;

namespace Aro.Common.Application.Shared.Metadata;

public class CountryMetadataProvider : ICountryMetadataProvider
{
    private readonly ICollection<CountryMetadata> _countries;
    public IReadOnlyCollection<CountryMetadata> Countries => (IReadOnlyCollection<CountryMetadata>)_countries;

    public CountryMetadataProvider()
    {
        _countries = LoadEmbeddedMetadata();
    }

    private static ICollection<CountryMetadata> LoadEmbeddedMetadata()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("Aro.Common.Application.Shared.Metadata.CountryMetadata.json")
                     ?? throw new InvalidOperationException("CountryMetadata.json not found");

        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true
        };

        return JsonSerializer.Deserialize<ICollection<CountryMetadata>>(json, options)
               ?? [];
    }

    //public bool TryGetCountry(string iso2, out CountryMetadata? info)
    //    => _countries.TryGetValue(iso2, out info);
}
