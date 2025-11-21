using Aro.Common.Application.Services;
using Aro.Common.Application.Shared;

namespace Aro.Common.Application.Services.LogManager;

public interface ILogManager : IService
{
    void LogInfo(string messageTemplate, params object[] propertyValues);
    void LogWarn(string messageTemplate, params object[] propertyValues);
    void LogDebug(string messageTemplate, params object[] propertyValues);
    void LogError(string messageTemplate, params object[] propertyValues);
    void LogError(Exception exception, string messageTemplate, params object[] propertyValues);
}
