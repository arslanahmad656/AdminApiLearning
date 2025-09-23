namespace Aro.Admin.Application.Services.DataServices;

public interface IAuditService
{
    Task LogApplicationSeeded(CancellationToken cancellationToken = default);

    Task LogMigrationsCompleted(CancellationToken cancellationToken = default);
}
