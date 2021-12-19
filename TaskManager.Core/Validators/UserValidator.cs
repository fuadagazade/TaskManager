using FluentValidation;
using FluentValidation.Validators;
using TaskManager.Core.Models;

namespace TaskManager.Core.Validators;
public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.FirstName)
            .NotNull().WithMessage("FirstName is null")
            .NotEmpty().WithMessage("FirstName is empty")
            .Length(1, 150).WithMessage("Minimum lenght of FirstName is 1, Maximum is 150");

        RuleFor(x => x.LastName)
            .NotNull().WithMessage("LastName is null")
            .NotEmpty().WithMessage("LastName is empty")
            .Length(1, 150).WithMessage("Minimum lenght of LastName is 1, Maximum is 150");

        RuleFor(x => x.Email)
            .NotNull().WithMessage("Email is null")
            .NotEmpty().WithMessage("Email is empty")
            .Length(1, 200).WithMessage("Minimum lenght of Email is 1, Maximum is 200")
            .EmailAddress(EmailValidationMode.AspNetCoreCompatible).WithMessage("Wrong Email");

        RuleFor(x => x.Password)
            .NotNull().WithMessage("Password is null")
            .NotEmpty().WithMessage("Password is empty")
            .Length(6, 30).WithMessage("Minimum lenght of Password is 6, Maximum is 30")
            .Matches(@"^(?=.*\d)(?=.*[a-zA-Z]).{6,30}$").WithMessage("Only allow passwords with 6 or more alphanumeric characters");
    }
}