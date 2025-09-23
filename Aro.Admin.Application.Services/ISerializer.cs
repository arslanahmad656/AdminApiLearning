namespace Aro.Admin.Application.Services;

public interface ISerializer
{
    string Serialize<T>(T obj, bool pretty = false);
    T? Deserialize<T>(string json);
}
