using Aro.Admin.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Admin.Presentation.Api.Validators;

public class GetBootstrapUserModelValidator : AbstractValidator<GetBootstrapUserModel>
{
    public GetBootstrapUserModelValidator()
    {
        RuleFor(m => m.BootstrapPassword)
            .NotNull()
            .NotEmpty();
    }
}
