using MediatR;

namespace Aro.Admin.Application.Mediator.SystemSettings.Queries;

public record IsSystemInitializedQuery : IRequest<bool>;

