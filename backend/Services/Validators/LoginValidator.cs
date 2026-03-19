using FluentValidation;
using HouseholdBudgetApi.DTOs.Auth;

namespace HouseholdBudgetApi.Services.Validators;

/// <summary>
/// Fluent validation rules for user login requests.
/// </summary>
public class LoginValidator : AbstractValidator<LoginRequestDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
