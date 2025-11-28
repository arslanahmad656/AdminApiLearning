using Aro.Common.Application.Services.Metadata;
using Aro.Common.Domain.Shared.Exceptions;

namespace Aro.Common.Infrastructure.Services;

public partial class CountryMetadataService
{
    private List<CountryMetadata> Load()
    {
        if (!File.Exists(_jsonFile))
            throw new FileNotFoundException(_errorCodes.FILE_NOT_FOUND_ERROR, $"Not found: {_jsonFile}");

        var json = File.ReadAllText(_jsonFile);
        return _serializer.Deserialize<List<CountryMetadata>>(json)
            ?? throw new AroException(_errorCodes.DESERIALIZATION_ERROR, "Failed to parse metadata.");
    }
}

