
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffDb.Data;
using StaffDb.DTOs;
using StaffDb.Filters;
using StaffDb.Models;

namespace StaffDb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController(
    AppDbContext context) : ControllerBase
{
    // GET: api/departments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentResponse>>> GetDepartments(
        CancellationToken cancellationToken)
    {
        var departments = await context.Departments
            .AsNoTracking()
            .OrderBy(department => department.Id)
            .Select(department => new DepartmentResponse
            {
                Id = department.Id,
                Name = department.Name
            })
            .ToListAsync(cancellationToken);

        return Ok(departments);
    }

    // GET: api/departments/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartmentResponse>> GetDepartment(
        int id,
        CancellationToken cancellationToken)
    {
        var department = await context.Departments
            .AsNoTracking()
            .Where(department => department.Id == id)
            .Select(department => new DepartmentResponse
            {
                Id = department.Id,
                Name = department.Name
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (department is null)
        {
            return NotFound("Department not found.");
        }

        return Ok(department);
    }

    // POST: api/departments
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilter))]
    public async Task<ActionResult<DepartmentResponse>> CreateDepartment(
        DepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        // Check whether department name already exists.
        var departmentExists = await context.Departments
            .AnyAsync(
                department => department.Name == name,
                cancellationToken);

        if (departmentExists)
        {
            return Conflict("A department with this name already exists.");
        }

        var department = new Department
        {
            Name = name
        };

        context.Departments.Add(department);

        await context.SaveChangesAsync(cancellationToken);

        var response = new DepartmentResponse
        {
            Id = department.Id,
            Name = department.Name
        };

        return CreatedAtAction(
            nameof(GetDepartment),
            new { id = department.Id },
            response);
    }

    // PUT: api/departments/5
    [HttpPut("{id:int}")]
    [ServiceFilter(typeof(ValidationFilter))]
    public async Task<ActionResult<DepartmentResponse>> UpdateDepartment(
        int id,
        DepartmentRequest request,
        CancellationToken cancellationToken)
    {
        // Find the existing department.
        var department = await context.Departments
            .FirstOrDefaultAsync(
                department => department.Id == id,
                cancellationToken);

        if (department is null)
        {
            return NotFound("Department not found.");
        }

        var name = request.Name.Trim();

        // Check for duplicate name, excluding the current department.
        var nameExists = await context.Departments
            .AnyAsync(
                department =>
                    department.Name == name &&
                    department.Id != id,
                cancellationToken);

        if (nameExists)
        {
            return Conflict("Another department already uses this name.");
        }

        department.Name = name;

        await context.SaveChangesAsync(cancellationToken);

        var response = new DepartmentResponse
        {
            Id = department.Id,
            Name = department.Name
        };

        return Ok(response);
    }

    // DELETE: api/departments/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteDepartment(
        int id,
        CancellationToken cancellationToken)
    {
        var department = await context.Departments
            .FirstOrDefaultAsync(
                department => department.Id == id,
                cancellationToken);

        if (department is null)
        {
            return NotFound("Department not found.");
        }

        // Check whether employees are assigned to this department.
        var hasEmployees = await context.Employees
            .AnyAsync(
                employee => employee.DepartmentId == id,
                cancellationToken);

        if (hasEmployees)
        {
            return Conflict(
                "This department cannot be deleted because employees are assigned to it.");
        }

        context.Departments.Remove(department);

        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
