using FluentValidation;
using TaskManager.Core.Models;

namespace TaskManager.Core.Validators;
public class OrganizationValidator : AbstractValidator<Organization>
{
    public OrganizationValidator()
    {
        RuleFor(x => x.Name)
            .NotNull().WithMessage("Name is null")
            .NotEmpty().WithMessage("Name is empty")
            .Length(1, 150).WithMessage("Minimum lenght of Name is 1, Maximum is 150");

        RuleFor(x=>x.Tag)
            .NotNull().WithMessage("Tag is null")
            .NotEmpty().WithMessage("Tag is empty")
            .Length(1, 150).WithMessage("Minimum lenght of Tag is 1, Maximum is 150");
    }
}