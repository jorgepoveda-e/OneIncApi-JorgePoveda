using FluentValidation;
using OneIncApi.Services.DTOs;

namespace OneIncApi.Services.Validators
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(128).WithMessage("First name cannot exceed 128 characters");

            RuleFor(x => x.LastName)
                .MaximumLength(128).WithMessage("Last name cannot exceed 128 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

            RuleFor(x => x.DateOfBirth)
                .Must(dob => DateTime.UtcNow.AddYears(-18) >= dob)
                .WithMessage("User must be at least 18 years old");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\d{10}$").WithMessage("Phone number must be 10 digits");
        }
    }
}
