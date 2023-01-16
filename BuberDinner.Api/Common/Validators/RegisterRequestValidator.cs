using BuberDinner.Contract.Authentication;
using FluentValidation;

namespace BuberDinner.Api.Common.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName).Length(1, 3);
        RuleFor(x => x.LastName).Length(1, 50);
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Password).Length(1, 50);
    }
}