namespace Aro.Admin.Application.Shared.Options;

public record LoggingSettings
{
    public bool TrackTimeInDebugLevel { get; init; }
    public bool IncludeBodyInRequestLogging { get; init; }
}

