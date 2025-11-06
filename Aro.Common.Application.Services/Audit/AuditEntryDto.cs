namespace Aro.Common.Application.Services.Audit;

public record AuditEntryDto(string Action, string EntityType, string EntityId, string Payload);
