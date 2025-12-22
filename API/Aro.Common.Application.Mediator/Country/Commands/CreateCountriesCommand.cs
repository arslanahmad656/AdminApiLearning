using Aro.Common.Application.Mediator.Country.DTOs;
using MediatR;

namespace Aro.Common.Application.Mediator.Country.Commands;

public record CreateCountriesCommand(CreateCountriesRequest Data) : IRequest<CreateCountriesResponse>;

