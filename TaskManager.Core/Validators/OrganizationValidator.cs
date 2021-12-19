using FluentValidation;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;

namespace TaskManager.Core.Validators;
public class OrganizationValidator : AbstractValidator<Organization>
{
    public OrganizationValidator(IOrganizationService organizationService)
    {
        RuleFor(x => x.Name).Length(1, 150);

        RuleFor(x=>x.Tag).Length(1, 150).Custom((x, context) =>
        {
            if (organizationService.Exists(x) > 0)
            {
                context.AddFailure("Organization with this tag name exists");
            }
        }); ;
    }
}