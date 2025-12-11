using Aro.Booking.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Booking.Presentation.Api.Validators;

public class GetPolicyByPropertyRouteParametersValidator : AbstractValidator<GetPolicyByPropertyRouteParameters>
{
    public GetPolicyByPropertyRouteParametersValidator()
    {
        RuleFor(p => p.PropertyId)
            .NotEmpty();

        RuleFor(p => p.PolicyId)
            .NotEmpty();
    }
}

