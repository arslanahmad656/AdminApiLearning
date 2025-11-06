using Aro.Common.Application.Services;

namespace Aro.Admin.Application.Services.Serializer;

public interface ISerializer : IService
{
    string Serialize<T>(T obj, bool pretty = false);
    T? Deserialize<T>(string json);
}
