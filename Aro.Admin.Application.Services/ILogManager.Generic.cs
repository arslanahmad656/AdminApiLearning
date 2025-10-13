namespace Aro.Admin.Application.Services;

public interface ILogManager<T> : IService
{
    void LogInfo(string messageTemplate, params object[] propertyValues);
    void LogWarn(string messageTemplate, params object[] propertyValues);
    void LogDebug(string messageTemplate, params object[] propertyValues);
    void LogError(string messageTemplate, params object[] propertyValues);
    void LogError(Exception exception, string messageTemplate, params object[] propertyValues);
}
