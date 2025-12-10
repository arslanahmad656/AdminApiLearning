using Aro.Booking.Application.Mediator.Policy.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Policy.Queries;

public record GetPolicyQuery(GetPolicyRequest Data) : IRequest<GetPolicyResponse>;

