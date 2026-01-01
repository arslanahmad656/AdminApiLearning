using Aro.Booking.Application.Mediator.Policy.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Policy.Commands;

public record ReorderPoliciesCommand(ReorderPoliciesRequest Request) : IRequest<ReorderPoliciesResponse>;
