using Aro.Booking.Application.Mediator.Policy.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Policy.Queries;

public record GetPoliciesQuery(GetPoliciesRequest Data) : IRequest<GetPoliciesResponse>;

