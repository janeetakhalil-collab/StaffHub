
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffDb.Data;
using StaffDb.DTOs;
using StaffDb.Models;

namespace StaffDb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(
    AppDbContext context,
    IValidator<EmployeeRequest> validator) : ControllerBase
{
    // GET: api/employees
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetEmployees(
        CancellationToken cancellationToken)
    {
        var employees = await context.Employees
            .AsNoTracking()
            .OrderBy(employee => employee.Id)
            .Select(employee => new EmployeeResponse
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department.Name
            })
            .ToListAsync(cancellationToken);

        return Ok(employees);
    }

    // GET: api/employees/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeResponse>> GetEmployee(
        int id,
        CancellationToken cancellationToken)
    {
        var employee = await context.Employees
            .AsNoTracking()
            .Where(employee => employee.Id == id)
            .Select(employee => new EmployeeResponse
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department.Name
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (employee is null)
        {
            return NotFound(new
            {
                message = "Employee not found."
            });
        }

        return Ok(employee);
    }

    // POST: api/employees
    [HttpPost]
    public async Task<ActionResult<EmployeeResponse>> CreateEmployee(
        EmployeeRequest request,
        CancellationToken cancellationToken)
    {
        // Validate employee request.
        var validationResult = await validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(error => error.ErrorMessage)
                        .ToArray());

            return BadRequest(new
            {
                message = "Validation failed.",
                errors
            });
        }

        var name = request.Name.Trim();
        var email = request.Email.Trim();

        // Check whether the department exists.
        var department = await context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(
                department => department.Id == request.DepartmentId,
                cancellationToken);

        if (department is null)
        {
            return BadRequest(new
            {
                message = "The selected department does not exist."
            });
        }

        // Check whether the email already exists.
        var emailExists = await context.Employees
            .AnyAsync(
                employee => employee.Email == email,
                cancellationToken);

        if (emailExists)
        {
            return Conflict(new
            {
                message = "An employee with this email already exists."
            });
        }

        var employee = new Employee
        {
            Name = name,
            Email = email,
            DepartmentId = department.Id
        };

        context.Employees.Add(employee);

        await context.SaveChangesAsync(cancellationToken);

        var response = new EmployeeResponse
        {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.Email,
            DepartmentId = employee.DepartmentId,
            DepartmentName = department.Name
        };

        return CreatedAtAction(
            nameof(GetEmployee),
            new { id = employee.Id },
            response);
    }

    // PUT: api/employees/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateEmployee(
        int id,
        EmployeeRequest request,
        CancellationToken cancellationToken)
    {
        // Validate employee request.
        var validationResult = await validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(error => error.ErrorMessage)
                        .ToArray());

            return BadRequest(new
            {
                message = "Validation failed.",
                errors
            });
        }

        // Find the existing employee.
        var employee = await context.Employees
            .FirstOrDefaultAsync(
                employee => employee.Id == id,
                cancellationToken);

        if (employee is null)
        {
            return NotFound(new
            {
                message = "Employee not found."
            });
        }

        // Check whether the selected department exists.
        var departmentExists = await context.Departments
            .AnyAsync(
                department => department.Id == request.DepartmentId,
                cancellationToken);

        if (!departmentExists)
        {
            return BadRequest(new
            {
                message = "The selected department does not exist."
            });
        }

        var name = request.Name.Trim();
        var email = request.Email.Trim();

        // Check for duplicate email, excluding the current employee.
        var duplicateEmail = await context.Employees
            .AnyAsync(
                existing =>
                    existing.Id != id &&
                    existing.Email == email,
                cancellationToken);

        if (duplicateEmail)
        {
            return Conflict(new
            {
                message = "An employee with this email already exists."
            });
        }

        employee.Name = name;
        employee.Email = email;
        employee.DepartmentId = request.DepartmentId;

        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    // DELETE: api/employees/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEmployee(
        int id,
        CancellationToken cancellationToken)
    {
        var employee = await context.Employees
            .FirstOrDefaultAsync(
                employee => employee.Id == id,
                cancellationToken);

        if (employee is null)
        {
            return NotFound(new
            {
                message = "Employee not found."
            });
        }

        context.Employees.Remove(employee);

        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
