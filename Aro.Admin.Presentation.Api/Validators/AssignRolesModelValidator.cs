using Aro.Admin.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Admin.Presentation.Api.Validators;

public class AssignRolesModelValidator : AbstractValidator<AssignRolesModel>
{
    public AssignRolesModelValidator()
    {
        RuleFor(m => m.RoleIds)
            .NotNull()
            .NotEmpty();

        RuleFor(m => m.UserIds)
            .NotNull()
            .NotEmpty();

        RuleForEach(m => m.RoleIds)
            .NotEqual(Guid.Empty);

        RuleForEach(m => m.UserIds)
            .NotEqual(Guid.Empty);
    }
}
