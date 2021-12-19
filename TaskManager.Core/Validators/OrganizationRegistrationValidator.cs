using FluentValidation;
using FluentValidation.Validators;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.ViewModels;

namespace TaskManager.Core.Validators;
public class OrganizationRegistrationValidator : AbstractValidator<OrganizationRegistration>
{
    public OrganizationRegistrationValidator(IOrganizationService organizationService, IUserService userService)
    {

        RuleFor(x => x.Name).Length(1, 150);

        RuleFor(x => x.Tag)
            .Length(1, 150)
            .Custom((x, context) =>
            {
                if (organizationService.Exists(x) > 0)
                {
                    context.AddFailure("Organization with this tag name exists");
                }
            });

        RuleFor(x => x.FirstName).Length(1, 150);

        RuleFor(x => x.LastName).Length(1, 150);

        RuleFor(x => x.Email).Length(1, 200).EmailAddress(EmailValidationMode.AspNetCoreCompatible).Custom((x, context) =>
        {
            if (userService.Exists(x, context.InstanceToValidate.Tag) > 0)
            {
                context.AddFailure("User with this email exists");
            }
        });

        RuleFor(x => x.Password).Length(6, 30).Matches(@"^(?=.*\d)(?=.*[a-zA-Z]).{6,30}$").WithMessage("Only allow passwords with 6 or more alphanumeric characters");
    }
}