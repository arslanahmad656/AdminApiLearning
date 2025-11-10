using MediatR;

namespace Aro.Admin.Application.Mediator.Migration.Commands;

/// <summary>
/// Command to apply the migrations on the database.
/// </summary>
public record MigrateDatabaseCommand() : IRequest;
