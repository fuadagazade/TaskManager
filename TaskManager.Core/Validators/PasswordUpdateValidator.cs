using FluentValidation;
using TaskManager.Core.ViewModels;

namespace TaskManager.Core.Validators;
public class PasswordUpdateValidator : AbstractValidator<PasswordUpdate>
{
    public PasswordUpdateValidator()
    {

        RuleFor(x => x.Current).Length(6, 30).Matches(@"^(?=.*\d)(?=.*[a-zA-Z]).{6,30}$").WithMessage("Only allow passwords with 6 or more alphanumeric characters");
        RuleFor(x => x.Changed).Length(6, 30).Matches(@"^(?=.*\d)(?=.*[a-zA-Z]).{6,30}$").WithMessage("Only allow passwords with 6 or more alphanumeric characters");

    }
}
