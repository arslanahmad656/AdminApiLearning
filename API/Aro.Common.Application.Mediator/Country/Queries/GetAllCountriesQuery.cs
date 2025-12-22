using Aro.Common.Application.Mediator.Country.DTOs;
using MediatR;

namespace Aro.Common.Application.Mediator.Country.Queries;

public record GetAllCountriesQuery : IRequest<GetAllCountriesResponse>;
