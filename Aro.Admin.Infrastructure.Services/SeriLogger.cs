using Aro.Admin.Application.Services;
using Serilog;

namespace Aro.Admin.Infrastructure.Services;

public class SeriLogger : ILogManager
{
    private readonly ILogger logger = Log.Logger;

    public void LogDebug(string messageTemplate, params object[] propertyValues) => logger.Debug(messageTemplate, propertyValues);

    public void LogError(string messageTemplate, params object[] propertyValues) => logger.Error(messageTemplate, propertyValues);

    public void LogError(Exception exception, string messageTemplate, params object[] propertyValues) => logger.Error(exception, messageTemplate, propertyValues);

    public void LogInfo(string messageTemplate, params object[] propertyValues) => logger.Information(messageTemplate, propertyValues);

    public void LogWarn(string messageTemplate, params object[] propertyValues) => logger.Warning(messageTemplate, propertyValues);
}
