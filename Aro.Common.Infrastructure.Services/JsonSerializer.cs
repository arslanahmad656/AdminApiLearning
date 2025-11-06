using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.Serializer;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Aro.Common.Infrastructure.Services;

public class JsonSerializer : ISerializer
{
    private readonly JsonSerializerOptions defaultOptions;
    private readonly JsonSerializerOptions prettyOptions;
    private readonly ILogManager<JsonSerializer> logger;

    public JsonSerializer(ILogManager<JsonSerializer> logger)
    {
        defaultOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNameCaseInsensitive = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        prettyOptions = new JsonSerializerOptions(defaultOptions)
        {
            PropertyNamingPolicy = null,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNameCaseInsensitive = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };

        this.logger = logger;
    }

    public string Serialize<T>(T obj, bool pretty = false)
    {
        logger.LogDebug("Starting {MethodName}", nameof(Serialize));

        if (obj == null)
        {
            logger.LogWarn("Attempted to serialize null object");
            throw new ArgumentNullException(nameof(obj));
        }

        var options = pretty ? prettyOptions : defaultOptions;
        logger.LogDebug("Using {OptionsType} options for serialization", pretty ? "pretty" : "default");

        var result = System.Text.Json.JsonSerializer.Serialize(obj, options);
        logger.LogDebug("Object serialized successfully, resultLength: {Length}", result.Length);

        logger.LogDebug("Completed {MethodName}", nameof(Serialize));
        return result;
    }

    public T? Deserialize<T>(string json)
    {
        logger.LogDebug("Starting {MethodName}", nameof(Deserialize));

        if (string.IsNullOrWhiteSpace(json))
        {
            logger.LogWarn("Attempted to deserialize null or empty JSON string");
            throw new ArgumentException("JSON string cannot be null or empty", nameof(json));
        }

        var result = System.Text.Json.JsonSerializer.Deserialize<T>(json, defaultOptions);
        logger.LogDebug("JSON deserialized successfully to type: {Type}", typeof(T).Name);

        logger.LogDebug("Completed {MethodName}", nameof(Deserialize));
        return result;
    }
}