using FluentValidation;
using TaskManager.Core.Interfaces.Services;
using Task = TaskManager.Core.Models.Task;

namespace TaskManager.Core.Validators;
public class TaskValidator : AbstractValidator<Task>
{
    public TaskValidator(IOrganizationService organizationService, IUserService userService)
    {
            RuleFor(x => x.Title).Length(1, 250);

            RuleFor(x => x.OrganizationId).Custom((x, context) =>
            {
                if (!organizationService.Exists(x))
                {
                    context.AddFailure("Organization not exists");
                }
            });

            RuleFor(x => x.CreatorId).Custom((x, context) =>
            {
                if (!userService.Exists(x))
                {
                    context.AddFailure("User not exists");
                }
            }); ;
        }
    }
}