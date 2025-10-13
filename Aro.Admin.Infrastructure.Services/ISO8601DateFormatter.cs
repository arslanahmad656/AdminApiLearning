﻿using Aro.Admin.Application.Services;

namespace Aro.Admin.Infrastructure.Services;

public class ISO8601DateFormatter(ILogManager<ISO8601DateFormatter> logger) : IDateFormatter
{
    public string Format(DateTimeOffset date)
    {
        logger.LogDebug("Starting {MethodName}", nameof(Format));
        
        var formatted = date.ToString("O");
        logger.LogDebug("Date formatted successfully: {FormattedDate}", formatted);
        
        logger.LogDebug("Completed {MethodName}", nameof(Format));
        return formatted;
    }
}
