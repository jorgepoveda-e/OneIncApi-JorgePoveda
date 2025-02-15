using FluentValidation;
using OneIncApi.Services.DTOs;

namespace OneIncApi.Services.Validators
{
    public class CreateUserValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(128);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(255);

            RuleFor(x => x.DateOfBirth)
                .Must(dob => DateTime.Now.AddYears(-18) >= dob)
                .WithMessage("User must be at least 18 years old");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\d{10}$")
                .MaximumLength(10)
                .WithMessage("Invalid phone number format");
        }
    }
}
