using Aro.Admin.Application.Services.DateFormatter;
using Aro.Common.Application.Services.LogManager;

namespace Aro.Admin.Infrastructure.Services;

public class MultiFormatter(ILogManager<MultiFormatter> logger) : IMultiFormatter
{
    public string Format(DateTimeOffset date)
    {
        logger.LogDebug("Starting {MethodName}", nameof(Format));
        
        var formatted = date.ToString("O");
        logger.LogDebug("Date formatted successfully: {FormattedDate}", formatted);
        
        logger.LogDebug("Completed {MethodName}", nameof(Format));
        return formatted;
    }

    public string FormatDate(DateOnly date)
    {
        var formatted = date.ToString("yyyy-MM-dd");
        return formatted;
    }

    public string FormatTime(TimeOnly time)
    {
        var formatted = time.ToString("HH:mm:ss");
        return formatted;
    }

    public string FormatGuidCompact(Guid guid)
    {
        var formatted = guid.ToString("N");
        return formatted;
    }
}
