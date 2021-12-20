using FluentValidation;
using FluentValidation.Validators;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;

namespace TaskManager.Core.Validators;
public class AuthenticationValidator : AbstractValidator<Authentication>
{
    public AuthenticationValidator(IOrganizationService organizationService)
    {
        RuleFor(x => x.Organization).Length(1, 150).Custom((x, context) =>
        {
            if (organizationService.Exists(x) == 0)
            {
                context.AddFailure("Organization with this tag name not exists");
            }
        });
        RuleFor(x => x.Email).Length(1, 200).EmailAddress(EmailValidationMode.AspNetCoreCompatible);

    }
}
