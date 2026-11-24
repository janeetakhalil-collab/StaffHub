
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffDb.Data;
using StaffDb.DTOs;
using StaffDb.Filters;
using StaffDb.Mappers;
using StaffDb.Models;

namespace StaffDb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(
    AppDbContext context) : ControllerBase
{
    // GET: api/employees
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetEmployees(
        CancellationToken cancellationToken)
    {
        var employees = await context.Employees
            .AsNoTracking()
            .Include(employee => employee.Department)
            .OrderBy(employee => employee.Id)
            .ToListAsync(cancellationToken);

        var response = employees
            .Select(EmployeeMapper.ToResponse)
            .ToList();

        return Ok(response);
    }

    // GET: api/employees/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeResponse>> GetEmployee(
        int id,
        CancellationToken cancellationToken)
    {
        var employee = await context.Employees
            .AsNoTracking()
            .Include(employee => employee.Department)
            .FirstOrDefaultAsync(
                employee => employee.Id == id,
                cancellationToken);

        if (employee is null)
            return NotFound("Employee not found.");

        return Ok(EmployeeMapper.ToResponse(employee));
    }

    // POST: api/employees
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilter))]
    public async Task<ActionResult<EmployeeResponse>> CreateEmployee(
        EmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var department = await context.Departments
            .FirstOrDefaultAsync(
                department => department.Id == request.DepartmentId,
                cancellationToken);

        if (department is null)
            return BadRequest("Department not found.");

        var email = request.Email.Trim();

        var emailExists = await context.Employees
            .AnyAsync(
                employee => employee.Email == email,
                cancellationToken);

        if (emailExists)
            return Conflict(
                "An employee with this email already exists.");

        var employee = new Employee
        {
            Name = request.Name.Trim(),
            Email = email,
            DepartmentId = department.Id,
            Department = department
        };

        context.Employees.Add(employee);

        await context.SaveChangesAsync(cancellationToken);

        var response = EmployeeMapper.ToResponse(employee);

        return CreatedAtAction(
            nameof(GetEmployee),
            new { id = employee.Id },
            response);
    }

    // PUT: api/employees/1
    [HttpPut("{id:int}")]
    [ServiceFilter(typeof(ValidationFilter))]
    public async Task<IActionResult> UpdateEmployee(
        int id,
        EmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var employee = await context.Employees
            .FirstOrDefaultAsync(
                employee => employee.Id == id,
                cancellationToken);

        if (employee is null)
            return NotFound("Employee not found.");

        var department = await context.Departments
            .FirstOrDefaultAsync(
                department => department.Id == request.DepartmentId,
                cancellationToken);

        if (department is null)
            return BadRequest("Department not found.");

        var email = request.Email.Trim();

        var emailExists = await context.Employees
            .AnyAsync(
                employee =>
                    employee.Email == email &&
                    employee.Id != id,
                cancellationToken);

        if (emailExists)
            return Conflict(
                "Another employee already uses this email.");

        employee.Name = request.Name.Trim();
        employee.Email = email;
        employee.DepartmentId = department.Id;
        employee.Department = department;

        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    // DELETE: api/employees/1
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
            return NotFound("Employee not found.");

        context.Employees.Remove(employee);

        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
