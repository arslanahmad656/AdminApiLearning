namespace Aro.Admin.Application.Shared.Options;

public record JwtOptions
{
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string Key { get; init; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; init; }
    public bool ValidateIssuer { get; init; }
    public bool ValidateAudience { get; init; }
    public bool ValidateLifetime { get; init; }
    public bool ValidateIssuerSigningKey { get; init; }
    public int RefreshTokenExpirationHours { get; init; }
    public int AccessTokenTrackingMarginInMinutes { get; init; }
}
