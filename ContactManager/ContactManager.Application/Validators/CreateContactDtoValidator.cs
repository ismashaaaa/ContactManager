using ContactManager.Application.DTOs.Contacts;
using FluentValidation;

namespace ContactManager.Application.Validators;

public class CreateContactDtoValidator : AbstractValidator<CreateContactDto>
{
    public CreateContactDtoValidator()
    {
        RuleFor(contact => contact.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name can't be longer than 50 characters.");

        RuleFor(contact => contact.DateOfBirth)
             .NotEmpty().WithMessage("Date of birth is required.")
             .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past.")
             .GreaterThan(DateTime.Today.AddYears(-90)).WithMessage("Age cannot be more than 90 years.")
             .LessThan(DateTime.Today.AddYears(-18)).WithMessage("Contact must be at least 18 years old.");

        RuleFor(contact => contact.Phone)
            .NotEmpty().WithMessage("Phone is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone format.");

        RuleFor(contact => contact.Salary)
            .GreaterThan(0).WithMessage("Salary must be greater than 0.");
    }
}