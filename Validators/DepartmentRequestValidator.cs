using FluentValidation;
using StaffDb.DTOs;

namespace StaffDb.Validators;

public class DepartmentRequestValidator
    : AbstractValidator<DepartmentRequest>
{
    public DepartmentRequestValidator()
    {
        RuleFor(request => request.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Department name is required.")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Department name is required.")
            .Must(name =>
                name.Trim().Length >= 2 &&
                name.Trim().Length <= 100)
            .WithMessage(
                "Department name must contain 2 to 100 characters.");
    }
}
