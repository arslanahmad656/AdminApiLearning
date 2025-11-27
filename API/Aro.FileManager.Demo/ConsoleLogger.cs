using Aro.Common.Application.Services.LogManager;
using Serilog;

namespace Aro.FileManager.Demo;

public class ConsoleLogger<T> : ILogManager<T>
{
    private readonly ILogger _logger = Log.ForContext<T>();

    public void LogDebug(string messageTemplate, params object[] propertyValues)
        => _logger.Debug(messageTemplate, propertyValues);

    public void LogError(string messageTemplate, params object[] propertyValues)
        => _logger.Error(messageTemplate, propertyValues);

    public void LogError(Exception exception, string messageTemplate, params object[] propertyValues)
        => _logger.Error(exception, messageTemplate, propertyValues);

    public void LogInfo(string messageTemplate, params object[] propertyValues)
        => _logger.Information(messageTemplate, propertyValues);

    public void LogWarn(string messageTemplate, params object[] propertyValues)
        => _logger.Warning(messageTemplate, propertyValues);
}

