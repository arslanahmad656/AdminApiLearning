using Aro.Admin.Application.Services;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Aro.Admin.Infrastructure.Services;

public class JsonSerializer : ISerializer
{
    private readonly JsonSerializerOptions defaultOptions;
    private readonly JsonSerializerOptions prettyOptions;

    public JsonSerializer()
    {
        defaultOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNameCaseInsensitive = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        prettyOptions = new JsonSerializerOptions(defaultOptions)
        {
            WriteIndented = true
        };
    }

    public string Serialize<T>(T obj, bool pretty = false)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        var options = pretty ? prettyOptions : defaultOptions;

        try
        {
            return System.Text.Json.JsonSerializer.Serialize(obj, options);
        }
        catch (Exception ex)
        {
            throw new SerializationException("Error occurred during serialization", ex);
        }
    }

    public T? Deserialize<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON string cannot be null or empty", nameof(json));

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json, defaultOptions);
        }
        catch (Exception ex)
        {
            throw new SerializationException("Error occurred during deserialization", ex);
        }
    }
}