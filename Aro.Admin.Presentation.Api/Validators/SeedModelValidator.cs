using Aro.Admin.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Admin.Presentation.Api.Validators;

public class SeedModelValidator : AbstractValidator<SeedModel>
{
    public SeedModelValidator()
    {
        RuleFor(m => m.FilePath)
            .NotNull()
            .NotEmpty();
    }
}
