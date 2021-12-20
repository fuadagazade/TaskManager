using FluentValidation;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.ViewModels;

namespace TaskManager.Core.Validators;
public class OrganizationUpdateValidator : AbstractValidator<OrganizationUpdate>
{
    public OrganizationUpdateValidator(IOrganizationService organizationService, IUserService userService)
    {
        RuleFor(x=>x.Id).NotEmpty().NotNull();
        RuleFor(x => x.Name).Length(1, 150);
    }
}