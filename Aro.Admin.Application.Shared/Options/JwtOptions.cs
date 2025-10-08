namespace Aro.Admin.Application.Shared.Options;

public record JwtOptions(string Issuer, string Audience, string Key, int AccessTokenExpirationMinutes, bool ValidateIssuer, bool ValidateAudience, bool ValidateLifetime, bool ValidateIssuerSigningKey);
