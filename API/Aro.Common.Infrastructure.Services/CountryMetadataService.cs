using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.Metadata;
using Aro.Common.Application.Services.Serializer;
using Aro.Common.Domain.Shared;

namespace Aro.Common.Infrastructure.Services;

public partial class CountryMetadataService : ICountryMetadataService
{
    private readonly ErrorCodes _errorCodes;
    private readonly ILogManager<CountryMetadataService> _logger;
    private readonly ISerializer _serializer;
    private readonly string _jsonFile;

    private readonly List<CountryMetadata> _countries = [];
    public IReadOnlyCollection<CountryMetadata> Countries => _countries;

    public CountryMetadataService(
        ErrorCodes errorCodes,
        ILogManager<CountryMetadataService> logger,
        ISerializer serializer,
        string jsonFile)
    {
        _errorCodes = errorCodes;
        _logger = logger;
        _serializer = serializer;
        _jsonFile = jsonFile;

        _countries.AddRange(Load());
    }
}
