using Aro.Common.Application.Shared;
namespace Aro.Common.Application.Services.Serializer;

public interface ISerializer : IService
{
    string Serialize<T>(T obj, bool pretty = false);
    T? Deserialize<T>(string json);
}
