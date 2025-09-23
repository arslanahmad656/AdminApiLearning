using Aro.Admin.Application.Mediator.Seed.Commands;
using Aro.Admin.Application.Services;
using MediatR;

namespace Aro.Admin.Application.Mediator.Seed.Handlers;

public class SeedApplicationCommandHandler
(
    ISeeder seedService
)
: IRequestHandler<SeedApplicationCommand>
{
    public async Task Handle(SeedApplicationCommand request, CancellationToken cancellationToken)
    {
        await seedService.Seed(request.JsonFilePath, cancellationToken).ConfigureAwait(false);
    }
}
