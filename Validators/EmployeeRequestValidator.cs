using FluentValidation;
using StaffDb.DTOs;

namespace StaffDb.Validators;

public class EmployeeRequestValidator
    : AbstractValidator<EmployeeRequest>
{
    public EmployeeRequestValidator()
    {
        // Employee Name validation
        RuleFor(request => request.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Employee name is required.")
            .Must(name =>
                name.Trim().Length >= 2 &&
                name.Trim().Length <= 100)
            .WithMessage(
                "Employee name must contain 2 to 100 characters.");

        // Employee Email validation
        RuleFor(request => request.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Enter a valid email address.")
            .MaximumLength(150)
            .WithMessage("Email cannot exceed 150 characters.");

        // Department ID validation
        RuleFor(request => request.DepartmentId)
            .GreaterThan(0)
            .WithMessage("Please provide a valid Department ID.");
    }
}